using Mes.Server.Database;
using Mes.Server.Models.Lot;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Mes.Server.Repositories.Lot;

public class LotRepository
    : ILotRepository
{
    private readonly MesDbConnectionFactory _connectionFactory;

    public LotRepository(
        MesDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    // =====================================================
    // LOT 코드 기준 단건 조회
    //
    // LOT 기본정보 +
    // Processed / Good / Defect 실적을 함께 조회
    // =====================================================
    public async Task<LotInfo?> GetByCodeAsync(
        string lotCode)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT
                l.lot_id,
                l.work_order_id,
                wo.work_order_code,
                l.lot_code,
                l.lot_sequence,
                l.target_qty,
                l.status,
                l.created_at,
                l.started_at,
                l.completed_at,

                COUNT(qr.quality_result_id)
                    AS processed_qty,

                SUM(
                    CASE
                        WHEN qr.result = 'PASS'
                        THEN 1
                        ELSE 0
                    END
                ) AS good_qty,

                SUM(
                    CASE
                        WHEN qr.result = 'FAIL'
                        THEN 1
                        ELSE 0
                    END
                ) AS defect_qty

            FROM lots l

            INNER JOIN work_orders wo
                ON l.work_order_id = wo.work_order_id

            LEFT JOIN products p
                ON l.lot_id = p.lot_id

            LEFT JOIN quality_results qr
                ON p.product_id = qr.product_id

            WHERE l.lot_code = @LotCode

            GROUP BY
                l.lot_id,
                l.work_order_id,
                wo.work_order_code,
                l.lot_code,
                l.lot_sequence,
                l.target_qty,
                l.status,
                l.created_at,
                l.started_at,
                l.completed_at;
            """;


        await using var command =
            new SqlCommand(sql, connection);


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


        return MapLot(reader);
    }


    // =====================================================
    // 특정 WorkOrder에 속한 전체 LOT 조회
    //
    // 기본적으로 5개 LOT이 반환되며
    // lot_sequence 순으로 정렬
    // =====================================================
    public async Task<IReadOnlyList<LotInfo>>
        GetByWorkOrderCodeAsync(
            string workOrderCode)
    {
        var lots = new List<LotInfo>();


        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT
                l.lot_id,
                l.work_order_id,
                wo.work_order_code,
                l.lot_code,
                l.lot_sequence,
                l.target_qty,
                l.status,
                l.created_at,
                l.started_at,
                l.completed_at,

                COUNT(qr.quality_result_id)
                    AS processed_qty,

                SUM(
                    CASE
                        WHEN qr.result = 'PASS'
                        THEN 1
                        ELSE 0
                    END
                ) AS good_qty,

                SUM(
                    CASE
                        WHEN qr.result = 'FAIL'
                        THEN 1
                        ELSE 0
                    END
                ) AS defect_qty

            FROM lots l

            INNER JOIN work_orders wo
                ON l.work_order_id = wo.work_order_id

            LEFT JOIN products p
                ON l.lot_id = p.lot_id

            LEFT JOIN quality_results qr
                ON p.product_id = qr.product_id

            WHERE wo.work_order_code = @WorkOrderCode

            GROUP BY
                l.lot_id,
                l.work_order_id,
                wo.work_order_code,
                l.lot_code,
                l.lot_sequence,
                l.target_qty,
                l.status,
                l.created_at,
                l.started_at,
                l.completed_at

            ORDER BY
                l.lot_sequence;
            """;


        await using var command =
            new SqlCommand(sql, connection);


        command.Parameters.Add(
            "@WorkOrderCode",
            SqlDbType.VarChar,
            30
        ).Value = workOrderCode;


        await using var reader =
            await command.ExecuteReaderAsync();


        while (await reader.ReadAsync())
        {
            lots.Add(
                MapLot(reader)
            );
        }


        return lots;
    }


    // =====================================================
    // 현재 RUNNING LOT 조회
    //
    // DB Unique Index에 의해
    // 동일 WorkOrder에는 RUNNING LOT이 최대 1개만 존재
    // =====================================================
    public async Task<LotInfo?> GetRunningAsync(
        string workOrderCode)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT
                l.lot_id,
                l.work_order_id,
                wo.work_order_code,
                l.lot_code,
                l.lot_sequence,
                l.target_qty,
                l.status,
                l.created_at,
                l.started_at,
                l.completed_at,

                COUNT(qr.quality_result_id)
                    AS processed_qty,

                SUM(
                    CASE
                        WHEN qr.result = 'PASS'
                        THEN 1
                        ELSE 0
                    END
                ) AS good_qty,

                SUM(
                    CASE
                        WHEN qr.result = 'FAIL'
                        THEN 1
                        ELSE 0
                    END
                ) AS defect_qty

            FROM lots l

            INNER JOIN work_orders wo
                ON l.work_order_id = wo.work_order_id

            LEFT JOIN products p
                ON l.lot_id = p.lot_id

            LEFT JOIN quality_results qr
                ON p.product_id = qr.product_id

            WHERE wo.work_order_code = @WorkOrderCode
              AND l.status = 'RUNNING'

            GROUP BY
                l.lot_id,
                l.work_order_id,
                wo.work_order_code,
                l.lot_code,
                l.lot_sequence,
                l.target_qty,
                l.status,
                l.created_at,
                l.started_at,
                l.completed_at;
            """;


        await using var command =
            new SqlCommand(sql, connection);


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


        return MapLot(reader);
    }


    // =====================================================
    // SqlDataReader → LotInfo 변환
    //
    // 반복되는 Mapping 코드를 하나로 분리
    // =====================================================
    private static LotInfo MapLot(
        SqlDataReader reader)
    {
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


        return new LotInfo
        {
            LotId =
                reader.GetInt64(0),

            WorkOrderId =
                reader.GetInt64(1),

            WorkOrderCode =
                reader.GetString(2),

            LotCode =
                reader.GetString(3),

            LotSequence =
                reader.GetInt16(4),

            TargetQty =
                reader.GetInt16(5),

            Status =
                reader.GetString(6),

            CreatedAt =
                new DateTimeOffset(
                    createdAtUtc
                ),

            StartedAt =
                startedAt,

            CompletedAt =
                completedAt,

            ProcessedQty =
                Convert.ToInt32(
                    reader["processed_qty"]
                ),

            GoodQty =
                Convert.ToInt32(
                    reader["good_qty"]
                ),

            DefectQty =
                Convert.ToInt32(
                    reader["defect_qty"]
                )
        };
    }
}