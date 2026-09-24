using System.Data;
using Mes.Server.Database;
using Mes.Server.Models.Product;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Repositories.Product;

public class ProductRepository
    : IProductRepository
{
    private readonly MesDbConnectionFactory _connectionFactory;

    public ProductRepository(
        MesDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    // =====================================================
    // Product 코드 기준 단건 조회
    //
    // Product의 생산상태 / 품질상태 /
    // 현재 위치 / LOT / WorkOrder까지 함께 조회
    // =====================================================
    public async Task<ProductInfo?> GetByCodeAsync(
        string productCode)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT
                p.product_id,
                p.product_code,

                p.lot_id,
                l.lot_code,

                l.work_order_id,
                wo.work_order_code,

                p.sequence_no,
                p.status,
                p.quality_status,

                p.current_location_id,
                loc.location_code,
                loc.location_name,

                p.created_at,
                p.started_at,
                p.completed_at

            FROM products p

            INNER JOIN lots l
                ON p.lot_id = l.lot_id

            INNER JOIN work_orders wo
                ON l.work_order_id = wo.work_order_id

            LEFT JOIN locations loc
                ON p.current_location_id = loc.location_id

            WHERE p.product_code = @ProductCode;
            """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        command.Parameters.Add(
            "@ProductCode",
            SqlDbType.VarChar,
            50
        ).Value = productCode;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            return null;
        }


        return MapProduct(reader);
    }


    // =====================================================
    // 특정 LOT의 전체 Product 조회
    //
    // LOT당 Product 60건을 sequence_no 순으로 조회
    // =====================================================
    public async Task<IReadOnlyList<ProductInfo>>
        GetByLotCodeAsync(
            string lotCode)
    {
        var products =
            new List<ProductInfo>();


        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT
                p.product_id,
                p.product_code,

                p.lot_id,
                l.lot_code,

                l.work_order_id,
                wo.work_order_code,

                p.sequence_no,
                p.status,
                p.quality_status,

                p.current_location_id,
                loc.location_code,
                loc.location_name,

                p.created_at,
                p.started_at,
                p.completed_at

            FROM products p

            INNER JOIN lots l
                ON p.lot_id = l.lot_id

            INNER JOIN work_orders wo
                ON l.work_order_id = wo.work_order_id

            LEFT JOIN locations loc
                ON p.current_location_id = loc.location_id

            WHERE l.lot_code = @LotCode

            ORDER BY p.sequence_no;
            """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        command.Parameters.Add(
            "@LotCode",
            SqlDbType.VarChar,
            40
        ).Value = lotCode;


        await using var reader =
            await command.ExecuteReaderAsync();


        while (await reader.ReadAsync())
        {
            products.Add(
                MapProduct(reader)
            );
        }


        return products;
    }


    // =====================================================
    // 해당 LOT에서 다음 생산 대상 Product 조회
    //
    // 아직 생산에 투입되지 않은 WAITING + PENDING 제품 중
    // sequence_no가 가장 작은 Product를 반환
    //
    // 이후 생산 진행 Service에서 다음 제품을 결정할 때 사용
    // =====================================================
    public async Task<ProductInfo?> GetNextWaitingAsync(
        string lotCode)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT TOP 1
                p.product_id,
                p.product_code,

                p.lot_id,
                l.lot_code,

                l.work_order_id,
                wo.work_order_code,

                p.sequence_no,
                p.status,
                p.quality_status,

                p.current_location_id,
                loc.location_code,
                loc.location_name,

                p.created_at,
                p.started_at,
                p.completed_at

            FROM products p

            INNER JOIN lots l
                ON p.lot_id = l.lot_id

            INNER JOIN work_orders wo
                ON l.work_order_id = wo.work_order_id

            LEFT JOIN locations loc
                ON p.current_location_id = loc.location_id

            WHERE l.lot_code = @LotCode

              AND p.status = 'WAITING'

              AND p.quality_status = 'PENDING'

            ORDER BY p.sequence_no;
            """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        command.Parameters.Add(
            "@LotCode",
            SqlDbType.VarChar,
            40
        ).Value = lotCode;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            return null;
        }


        return MapProduct(reader);
    }



    // =====================================================
    // 현재 품질판정 대상 Product 조회
    //
    // RUNNING WorkOrder
    // → RUNNING LOT
    // → quality_status = PENDING 중
    // → sequence_no가 가장 작은 Product
    //
    // Product status는 WAITING / IN_PROCESS를 제한하지 않음
    // =====================================================
    public async Task<ProductInfo?> GetCurrentQualityPendingAsync()
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
        DECLARE
            @RunningWorkOrderCount INT,
            @WorkOrderId BIGINT,
            @LotId BIGINT;


        ------------------------------------------------
        -- RUNNING WorkOrder 개수 확인
        ------------------------------------------------
        SELECT
            @RunningWorkOrderCount = COUNT(*)
        FROM work_orders
        WHERE status = 'RUNNING';


        IF @RunningWorkOrderCount = 0
        BEGIN
            RETURN;
        END;


        IF @RunningWorkOrderCount > 1
        BEGIN
            THROW 50401,
                'RUNNING 상태의 작업지시가 2개 이상 존재합니다.',
                1;
        END;


        ------------------------------------------------
        -- 현재 RUNNING WorkOrder
        ------------------------------------------------
        SELECT
            @WorkOrderId = work_order_id
        FROM work_orders
        WHERE status = 'RUNNING';


        ------------------------------------------------
        -- 현재 RUNNING LOT
        ------------------------------------------------
        SELECT
            @LotId = lot_id
        FROM lots
        WHERE work_order_id = @WorkOrderId
          AND status = 'RUNNING';


        IF @LotId IS NULL
        BEGIN
            THROW 50402,
                'RUNNING 작업지시에 RUNNING LOT이 없습니다.',
                1;
        END;


        ------------------------------------------------
        -- 품질판정 대기 Product
        ------------------------------------------------
        SELECT TOP 1

            p.product_id,
            p.product_code,

            p.lot_id,
            l.lot_code,

            l.work_order_id,
            wo.work_order_code,

            p.sequence_no,
            p.status,
            p.quality_status,

            p.current_location_id,
            loc.location_code,
            loc.location_name,

            p.created_at,
            p.started_at,
            p.completed_at

        FROM products p

        INNER JOIN lots l
            ON p.lot_id = l.lot_id

        INNER JOIN work_orders wo
            ON l.work_order_id = wo.work_order_id

        LEFT JOIN locations loc
            ON p.current_location_id = loc.location_id

        WHERE p.lot_id = @LotId

          AND p.quality_status = 'PENDING'

        ORDER BY p.sequence_no;
        """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            return null;
        }


        return MapProduct(reader);
    }


    // =====================================================
    // 현재 공정 진행 중 Product 조회
    //
    // Gateway 재시작 등의 상황에서
    // 현재 IN_PROCESS Product를 복구하기 위한 조회
    // =====================================================
    public async Task<ProductInfo?> GetCurrentInProcessAsync()
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
        DECLARE
            @RunningWorkOrderCount INT,
            @WorkOrderId BIGINT,
            @LotId BIGINT;


        SELECT
            @RunningWorkOrderCount = COUNT(*)
        FROM work_orders
        WHERE status = 'RUNNING';


        IF @RunningWorkOrderCount = 0
        BEGIN
            RETURN;
        END;


        IF @RunningWorkOrderCount > 1
        BEGIN
            THROW 50401,
                'RUNNING 상태의 작업지시가 2개 이상 존재합니다.',
                1;
        END;


        SELECT
            @WorkOrderId = work_order_id
        FROM work_orders
        WHERE status = 'RUNNING';


        SELECT
            @LotId = lot_id
        FROM lots
        WHERE work_order_id = @WorkOrderId
          AND status = 'RUNNING';


        IF @LotId IS NULL
        BEGIN
            THROW 50402,
                'RUNNING 작업지시에 RUNNING LOT이 없습니다.',
                1;
        END;


        SELECT TOP 1

            p.product_id,
            p.product_code,

            p.lot_id,
            l.lot_code,

            l.work_order_id,
            wo.work_order_code,

            p.sequence_no,
            p.status,
            p.quality_status,

            p.current_location_id,
            loc.location_code,
            loc.location_name,

            p.created_at,
            p.started_at,
            p.completed_at

        FROM products p

        INNER JOIN lots l
            ON p.lot_id = l.lot_id

        INNER JOIN work_orders wo
            ON l.work_order_id = wo.work_order_id

        LEFT JOIN locations loc
            ON p.current_location_id = loc.location_id

        WHERE p.lot_id = @LotId

          AND p.status = 'IN_PROCESS'

        ORDER BY p.sequence_no;
        """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            return null;
        }


        return MapProduct(reader);
    }


    // =====================================================
    // SqlDataReader → ProductInfo 변환
    // =====================================================
    private static ProductInfo MapProduct(
        SqlDataReader reader)
    {
        var createdAtUtc =
            DateTime.SpecifyKind(
                reader.GetDateTime(12),
                DateTimeKind.Utc
            );


        DateTimeOffset? startedAt = null;

        if (!reader.IsDBNull(13))
        {
            var startedAtUtc =
                DateTime.SpecifyKind(
                    reader.GetDateTime(13),
                    DateTimeKind.Utc
                );

            startedAt =
                new DateTimeOffset(
                    startedAtUtc
                );
        }


        DateTimeOffset? completedAt = null;

        if (!reader.IsDBNull(14))
        {
            var completedAtUtc =
                DateTime.SpecifyKind(
                    reader.GetDateTime(14),
                    DateTimeKind.Utc
                );

            completedAt =
                new DateTimeOffset(
                    completedAtUtc
                );
        }


        return new ProductInfo
        {
            ProductId =
                reader.GetInt64(0),

            ProductCode =
                reader.GetString(1),

            LotId =
                reader.GetInt64(2),

            LotCode =
                reader.GetString(3),

            WorkOrderId =
                reader.GetInt64(4),

            WorkOrderCode =
                reader.GetString(5),

            SequenceNo =
                reader.GetInt16(6),

            Status =
                reader.GetString(7),

            QualityStatus =
                reader.GetString(8),

            CurrentLocationId =
                reader.IsDBNull(9)
                    ? null
                    : reader.GetInt32(9),

            CurrentLocationCode =
                reader.IsDBNull(10)
                    ? null
                    : reader.GetString(10),

            CurrentLocationName =
                reader.IsDBNull(11)
                    ? null
                    : reader.GetString(11),

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
}