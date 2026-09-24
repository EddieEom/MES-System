using System.Data;
using Mes.Server.Database;
using Mes.Server.Models.WorkOrder;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Repositories.WorkOrder;

public class WorkOrderRepository
    : IWorkOrderRepository
{
    private readonly MesDbConnectionFactory _connectionFactory;

    public WorkOrderRepository(
        MesDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    // =====================================================
    // 작업지시 생성
    //
    // usp_CreateWorkOrder 호출
    //
    // WorkOrder 1건
    // → LOT 5건
    // → Product 300건
    //
    // 전체 작업은 Stored Procedure 내부 Transaction으로
    // 처리되므로 하나라도 실패하면 전체 Rollback
    // =====================================================
    public async Task<WorkOrderCreateResult> CreateAsync(
        WorkOrderCreateRequest request)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        // 저장프로시저 호출
        await using var command =
            new SqlCommand(
                "dbo.usp_CreateWorkOrder",
                connection
            );

        command.CommandType =
            CommandType.StoredProcedure;


        command.Parameters.Add(
            "@WorkOrderCode",
            SqlDbType.VarChar,
            30
        ).Value = request.WorkOrderCode;


        command.Parameters.Add(
            "@CreatedByUserId",
            SqlDbType.BigInt
        ).Value =
            request.CreatedByUserId.HasValue
                ? request.CreatedByUserId.Value
                : DBNull.Value;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException(
                "작업지시 생성 결과를 반환받지 못했습니다."
            );
        }


        return new WorkOrderCreateResult
        {
            WorkOrderId =
                Convert.ToInt64(
                    reader["work_order_id"]
                ),

            WorkOrderCode =
                reader["work_order_code"]
                    .ToString()
                ?? string.Empty,

            LotCount =
                Convert.ToInt32(
                    reader["lot_count"]
                ),

            ProductCount =
                Convert.ToInt32(
                    reader["product_count"]
                )
        };
    }


    // =====================================================
    // 작업지시 코드 기준 조회
    // =====================================================
    public async Task<WorkOrderInfo?> GetByCodeAsync(
        string workOrderCode)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT
                work_order_id,
                work_order_code,
                target_qty,
                lot_count,
                lot_size,
                status,
                created_by_user_id,
                created_at,
                started_at,
                completed_at

            FROM work_orders

            WHERE work_order_code = @WorkOrderCode;
            """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        command.Parameters.Add(
            "@WorkOrderCode",
            SqlDbType.VarChar,
            30
        ).Value = workOrderCode;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            return null;
        }


        var createdAtUtc =
            DateTime.SpecifyKind(
                reader.GetDateTime(7),
                DateTimeKind.Utc
            );


        DateTimeOffset? startedAt = null;

        if (!reader.IsDBNull(8))
        {
            var startedAtUtc =
                DateTime.SpecifyKind(
                    reader.GetDateTime(8),
                    DateTimeKind.Utc
                );

            startedAt =
                new DateTimeOffset(
                    startedAtUtc
                );
        }


        DateTimeOffset? completedAt = null;

        if (!reader.IsDBNull(9))
        {
            var completedAtUtc =
                DateTime.SpecifyKind(
                    reader.GetDateTime(9),
                    DateTimeKind.Utc
                );

            completedAt =
                new DateTimeOffset(
                    completedAtUtc
                );
        }


        return new WorkOrderInfo
        {
            WorkOrderId =
                reader.GetInt64(0),

            WorkOrderCode =
                reader.GetString(1),

            TargetQty =
                reader.GetInt32(2),

            LotCount =
                reader.GetInt16(3),

            LotSize =
                reader.GetInt16(4),

            Status =
                reader.GetString(5),

            CreatedByUserId =
                reader.IsDBNull(6)
                    ? null
                    : reader.GetInt64(6),

            CreatedAt =
                new DateTimeOffset(
                    createdAtUtc
                ),

            StartedAt =
                startedAt,

            CompletedAt =
                completedAt
        };
    }

    // 생산 작업지시 시작해주는 메서드
    public async Task<WorkOrderStartResult> StartAsync(
    string workOrderCode)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
        SET XACT_ABORT ON;

        BEGIN TRY

            BEGIN TRANSACTION;


            DECLARE
                @WorkOrderId BIGINT,
                @WorkOrderStatus VARCHAR(20),
                @LotId BIGINT;


            ------------------------------------------------
            -- WorkOrder 잠금 및 상태 확인
            ------------------------------------------------
            SELECT
                @WorkOrderId = work_order_id,
                @WorkOrderStatus = status

            FROM work_orders WITH (UPDLOCK, HOLDLOCK)

            WHERE work_order_code = @WorkOrderCode;


            IF @WorkOrderId IS NULL
            BEGIN
                THROW 50301,
                    '존재하지 않는 작업지시입니다.',
                    1;
            END;


            ------------------------------------------------
            -- IDLE 상태에서만 최초 START 허용
            ------------------------------------------------
            IF @WorkOrderStatus <> 'IDLE'
            BEGIN

                IF @WorkOrderStatus = 'RUNNING'
                BEGIN
                    THROW 50302,
                        '이미 실행 중인 작업지시입니다.',
                        1;
                END;


                IF @WorkOrderStatus = 'PAUSE'
                BEGIN
                    THROW 50303,
                        '일시정지된 작업지시는 RESUME을 사용해야 합니다.',
                        1;
                END;


                IF @WorkOrderStatus = 'COMPLETED'
                BEGIN
                    THROW 50304,
                        '완료된 작업지시는 다시 시작할 수 없습니다.',
                        1;
                END;


                THROW 50305,
                    '현재 상태에서는 작업을 시작할 수 없습니다.',
                    1;

            END;


            ------------------------------------------------
            -- 데이터 이상 여부 확인
            --
            -- IDLE WorkOrder인데 이미 RUNNING LOT이 있다면
            -- 상태 불일치이므로 시작 금지
            ------------------------------------------------
            IF EXISTS
            (
                SELECT 1
                FROM lots
                WHERE work_order_id = @WorkOrderId
                  AND status = 'RUNNING'
            )
            BEGIN
                THROW 50306,
                    '이미 RUNNING 상태의 LOT이 존재합니다.',
                    1;
            END;


            ------------------------------------------------
            -- 첫 WAITING LOT 선택
            ------------------------------------------------
            SELECT TOP 1
                @LotId = lot_id

            FROM lots WITH (UPDLOCK, HOLDLOCK)

            WHERE work_order_id = @WorkOrderId
              AND status = 'WAITING'

            ORDER BY lot_sequence;


            IF @LotId IS NULL
            BEGIN
                THROW 50307,
                    '시작할 수 있는 WAITING LOT이 없습니다.',
                    1;
            END;


            ------------------------------------------------
            -- WorkOrder START
            ------------------------------------------------
            UPDATE work_orders
            SET
                status = 'RUNNING',

                started_at =
                    COALESCE(
                        started_at,
                        SYSUTCDATETIME()
                    )

            WHERE work_order_id = @WorkOrderId;


            ------------------------------------------------
            -- 첫 LOT START
            ------------------------------------------------
            UPDATE lots
            SET
                status = 'RUNNING',

                started_at =
                    COALESCE(
                        started_at,
                        SYSUTCDATETIME()
                    )

            WHERE lot_id = @LotId;


            COMMIT TRANSACTION;


            ------------------------------------------------
            -- 결과 반환
            ------------------------------------------------
            SELECT
                wo.work_order_id,
                wo.work_order_code,
                wo.status
                    AS work_order_status,
                wo.started_at
                    AS work_order_started_at,

                l.lot_id,
                l.lot_code,
                l.status
                    AS lot_status,
                l.started_at
                    AS lot_started_at

            FROM work_orders wo

            INNER JOIN lots l
                ON wo.work_order_id =
                   l.work_order_id

            WHERE wo.work_order_id =
                  @WorkOrderId

              AND l.lot_id =
                  @LotId;

        END TRY

        BEGIN CATCH

            IF @@TRANCOUNT > 0
            BEGIN
                ROLLBACK TRANSACTION;
            END;

            THROW;

        END CATCH;
        """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        command.Parameters.Add(
            "@WorkOrderCode",
            SqlDbType.VarChar,
            30
        ).Value = workOrderCode;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException(
                "작업지시 시작 결과를 반환받지 못했습니다."
            );
        }


        var workOrderStartedAt =
            DateTime.SpecifyKind(
                reader.GetDateTime(3),
                DateTimeKind.Utc
            );


        var lotStartedAt =
            DateTime.SpecifyKind(
                reader.GetDateTime(7),
                DateTimeKind.Utc
            );


        return new WorkOrderStartResult
        {
            WorkOrderId =
                reader.GetInt64(0),

            WorkOrderCode =
                reader.GetString(1),

            WorkOrderStatus =
                reader.GetString(2),

            WorkOrderStartedAt =
                new DateTimeOffset(
                    workOrderStartedAt
                ),

            LotId =
                reader.GetInt64(4),

            LotCode =
                reader.GetString(5),

            LotStatus =
                reader.GetString(6),

            LotStartedAt =
                new DateTimeOffset(
                    lotStartedAt
                )
        };
    }

    // 생산 작업 중인 작업지시를 일시정지(PAUSE) 상태로 전환하는 메서드
    public async Task<WorkOrderStateResult> PauseAsync(
    string workOrderCode)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
        SET XACT_ABORT ON;

        BEGIN TRY

            BEGIN TRANSACTION;


            DECLARE
                @WorkOrderId BIGINT,
                @WorkOrderStatus VARCHAR(20),
                @CurrentLotId BIGINT;


            ------------------------------------------------
            -- WorkOrder 조회 및 잠금
            ------------------------------------------------
            SELECT
                @WorkOrderId = work_order_id,
                @WorkOrderStatus = status

            FROM work_orders WITH (UPDLOCK, HOLDLOCK)

            WHERE work_order_code = @WorkOrderCode;


            IF @WorkOrderId IS NULL
            BEGIN
                THROW 50311,
                    '존재하지 않는 작업지시입니다.',
                    1;
            END;


            ------------------------------------------------
            -- RUNNING 상태에서만 PAUSE 가능
            ------------------------------------------------
            IF @WorkOrderStatus <> 'RUNNING'
            BEGIN

                IF @WorkOrderStatus = 'IDLE'
                BEGIN
                    THROW 50312,
                        '시작되지 않은 작업지시는 일시정지할 수 없습니다.',
                        1;
                END;


                IF @WorkOrderStatus = 'PAUSE'
                BEGIN
                    THROW 50313,
                        '이미 일시정지된 작업지시입니다.',
                        1;
                END;


                IF @WorkOrderStatus = 'COMPLETED'
                BEGIN
                    THROW 50314,
                        '완료된 작업지시는 일시정지할 수 없습니다.',
                        1;
                END;


                THROW 50315,
                    '현재 상태에서는 일시정지할 수 없습니다.',
                    1;

            END;


            ------------------------------------------------
            -- 현재 RUNNING LOT 확인
            ------------------------------------------------
            SELECT
                @CurrentLotId = lot_id

            FROM lots WITH (UPDLOCK, HOLDLOCK)

            WHERE work_order_id = @WorkOrderId
              AND status = 'RUNNING';


            IF @CurrentLotId IS NULL
            BEGIN
                THROW 50316,
                    '현재 RUNNING 상태의 LOT이 없습니다.',
                    1;
            END;


            ------------------------------------------------
            -- WorkOrder PAUSE
            --
            -- LOT은 RUNNING 상태 유지
            ------------------------------------------------
            UPDATE work_orders
            SET
                status = 'PAUSE'

            WHERE work_order_id = @WorkOrderId;


            COMMIT TRANSACTION;


            ------------------------------------------------
            -- 결과 반환
            ------------------------------------------------
            SELECT
                wo.work_order_id,
                wo.work_order_code,
                wo.status
                    AS work_order_status,
                wo.started_at
                    AS work_order_started_at,

                l.lot_id,
                l.lot_code,
                l.status
                    AS current_lot_status

            FROM work_orders wo

            INNER JOIN lots l
                ON wo.work_order_id =
                   l.work_order_id

            WHERE wo.work_order_id =
                  @WorkOrderId

              AND l.lot_id =
                  @CurrentLotId;


        END TRY

        BEGIN CATCH

            IF @@TRANCOUNT > 0
            BEGIN
                ROLLBACK TRANSACTION;
            END;

            THROW;

        END CATCH;
        """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        command.Parameters.Add(
            "@WorkOrderCode",
            SqlDbType.VarChar,
            30
        ).Value = workOrderCode;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException(
                "작업지시 일시정지 결과를 반환받지 못했습니다."
            );
        }


        DateTimeOffset? startedAt = null;

        if (!reader.IsDBNull(3))
        {
            var startedAtUtc =
                DateTime.SpecifyKind(
                    reader.GetDateTime(3),
                    DateTimeKind.Utc
                );

            startedAt =
                new DateTimeOffset(
                    startedAtUtc
                );
        }


        return new WorkOrderStateResult
        {
            WorkOrderId =
                reader.GetInt64(0),

            WorkOrderCode =
                reader.GetString(1),

            WorkOrderStatus =
                reader.GetString(2),

            WorkOrderStartedAt =
                startedAt,

            CurrentLotId =
                reader.GetInt64(4),

            CurrentLotCode =
                reader.GetString(5),

            CurrentLotStatus =
                reader.GetString(6)
        };
    }

    // 생산 작업 중인 작업지시를 재개(RESUME) 상태로 전환하는 메서드
    public async Task<WorkOrderStateResult> ResumeAsync(
    string workOrderCode)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
        SET XACT_ABORT ON;

        BEGIN TRY

            BEGIN TRANSACTION;


            DECLARE
                @WorkOrderId BIGINT,
                @WorkOrderStatus VARCHAR(20),
                @CurrentLotId BIGINT;


            ------------------------------------------------
            -- WorkOrder 조회 및 잠금
            ------------------------------------------------
            SELECT
                @WorkOrderId = work_order_id,
                @WorkOrderStatus = status

            FROM work_orders WITH (UPDLOCK, HOLDLOCK)

            WHERE work_order_code = @WorkOrderCode;


            IF @WorkOrderId IS NULL
            BEGIN
                THROW 50321,
                    '존재하지 않는 작업지시입니다.',
                    1;
            END;


            ------------------------------------------------
            -- PAUSE 상태에서만 RESUME 가능
            ------------------------------------------------
            IF @WorkOrderStatus <> 'PAUSE'
            BEGIN

                IF @WorkOrderStatus = 'IDLE'
                BEGIN
                    THROW 50322,
                        '시작되지 않은 작업지시는 재개할 수 없습니다.',
                        1;
                END;


                IF @WorkOrderStatus = 'RUNNING'
                BEGIN
                    THROW 50323,
                        '이미 실행 중인 작업지시입니다.',
                        1;
                END;


                IF @WorkOrderStatus = 'COMPLETED'
                BEGIN
                    THROW 50324,
                        '완료된 작업지시는 재개할 수 없습니다.',
                        1;
                END;


                THROW 50325,
                    '현재 상태에서는 작업을 재개할 수 없습니다.',
                    1;

            END;


            ------------------------------------------------
            -- 기존 RUNNING LOT 확인
            --
            -- PAUSE 중에도 LOT은 RUNNING 유지
            ------------------------------------------------
            SELECT
                @CurrentLotId = lot_id

            FROM lots WITH (UPDLOCK, HOLDLOCK)

            WHERE work_order_id = @WorkOrderId
              AND status = 'RUNNING';


            IF @CurrentLotId IS NULL
            BEGIN
                THROW 50326,
                    '재개할 RUNNING LOT이 없습니다.',
                    1;
            END;


            ------------------------------------------------
            -- WorkOrder RESUME
            ------------------------------------------------
            UPDATE work_orders
            SET
                status = 'RUNNING'

            WHERE work_order_id = @WorkOrderId;


            COMMIT TRANSACTION;


            ------------------------------------------------
            -- 결과 반환
            ------------------------------------------------
            SELECT
                wo.work_order_id,
                wo.work_order_code,
                wo.status
                    AS work_order_status,
                wo.started_at
                    AS work_order_started_at,

                l.lot_id,
                l.lot_code,
                l.status
                    AS current_lot_status

            FROM work_orders wo

            INNER JOIN lots l
                ON wo.work_order_id =
                   l.work_order_id

            WHERE wo.work_order_id =
                  @WorkOrderId

              AND l.lot_id =
                  @CurrentLotId;


        END TRY

        BEGIN CATCH

            IF @@TRANCOUNT > 0
            BEGIN
                ROLLBACK TRANSACTION;
            END;

            THROW;

        END CATCH;
        """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        command.Parameters.Add(
            "@WorkOrderCode",
            SqlDbType.VarChar,
            30
        ).Value = workOrderCode;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException(
                "작업지시 재개 결과를 반환받지 못했습니다."
            );
        }


        DateTimeOffset? startedAt = null;

        if (!reader.IsDBNull(3))
        {
            var startedAtUtc =
                DateTime.SpecifyKind(
                    reader.GetDateTime(3),
                    DateTimeKind.Utc
                );

            startedAt =
                new DateTimeOffset(
                    startedAtUtc
                );
        }


        return new WorkOrderStateResult
        {
            WorkOrderId =
                reader.GetInt64(0),

            WorkOrderCode =
                reader.GetString(1),

            WorkOrderStatus =
                reader.GetString(2),

            WorkOrderStartedAt =
                startedAt,

            CurrentLotId =
                reader.GetInt64(4),

            CurrentLotCode =
                reader.GetString(5),

            CurrentLotStatus =
                reader.GetString(6)
        };
    }
}