using System.Data;
using Mes.Server.Database;
using Mes.Server.Models.Quality;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Repositories.Quality;

public class QualityResultRepository
    : IQualityResultRepository
{
    private readonly MesDbConnectionFactory _connectionFactory;

    public QualityResultRepository(
        MesDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    // =====================================================
    // 품질 결과 저장
    //
    // usp_RecordQualityResult 저장프로시저 호출
    //
    // 품질 결과 저장
    // → Product 품질상태 변경
    // → LOT 처리수량 확인
    // → LOT 완료 처리
    // → 다음 LOT RUNNING
    // → 전체 LOT 완료 시 WorkOrder 완료
    // =====================================================
    public async Task<QualityProcessResult> RecordAsync(
        QualityResultCreateRequest request)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        await using var command =
            new SqlCommand(
                "dbo.usp_RecordQualityResult",
                connection
            );

        command.CommandType =
            CommandType.StoredProcedure;


        // MQTT Event ID
        command.Parameters.Add(
            "@EventId",
            SqlDbType.UniqueIdentifier
        ).Value = request.EventId;


        // 대상 Product
        command.Parameters.Add(
            "@ProductId",
            SqlDbType.BigInt
        ).Value = request.ProductId;


        // PASS / FAIL
        command.Parameters.Add(
            "@Result",
            SqlDbType.VarChar,
            10
        ).Value = request.Result;


        // Gemini 원본 제품 유형
        command.Parameters.Add(
            "@SourceProductType",
            SqlDbType.NVarChar,
            50
        ).Value =
            request.SourceProductType is null
                ? DBNull.Value
                : request.SourceProductType;


        // UTC Timestamp
        command.Parameters.Add(
            "@InspectedAt",
            SqlDbType.DateTime2
        ).Value =
            request.InspectedAt.UtcDateTime;


        // GEMINI / PLC / MES
        command.Parameters.Add(
            "@SourceSystem",
            SqlDbType.VarChar,
            20
        ).Value = request.SourceSystem;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException(
                "품질 처리 결과를 반환받지 못했습니다."
            );
        }


        return new QualityProcessResult
        {
            WorkOrderCode =
                reader["work_order_code"]
                    .ToString()
                ?? string.Empty,

            WorkOrderStatus =
                reader["work_order_status"]
                    .ToString()
                ?? string.Empty,

            LotCode =
                reader["lot_code"]
                    .ToString()
                ?? string.Empty,

            LotStatus =
                reader["lot_status"]
                    .ToString()
                ?? string.Empty,

            ProcessedQty =
                Convert.ToInt32(
                    reader["processed_qty"]
                ),

            TargetQty =
                Convert.ToInt32(
                    reader["target_qty"]
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


    // =====================================================
    // EventId 기준 품질 결과 조회
    // =====================================================
    public async Task<QualityResult?> GetByEventIdAsync(
        Guid eventId)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT
                qr.quality_result_id,
                qr.event_id,
                qr.product_id,
                qr.machine_id,
                m.machine_code,
                qr.result,
                qr.source_product_type,
                qr.inspected_at,
                qr.source_system,
                qr.received_at

            FROM quality_results qr

            INNER JOIN machines m
                ON qr.machine_id = m.machine_id

            WHERE qr.event_id = @EventId;
            """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        command.Parameters.Add(
            "@EventId",
            SqlDbType.UniqueIdentifier
        ).Value = eventId;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            return null;
        }


        // DB DATETIME2에는 Offset 정보가 없기 때문에
        // 저장 정책에 따라 UTC로 간주
        var inspectedAtUtc =
            DateTime.SpecifyKind(
                reader.GetDateTime(7),
                DateTimeKind.Utc
            );

        var receivedAtUtc =
            DateTime.SpecifyKind(
                reader.GetDateTime(9),
                DateTimeKind.Utc
            );


        return new QualityResult
        {
            QualityResultId =
                reader.GetInt64(0),

            EventId =
                reader.GetGuid(1),

            ProductId =
                reader.GetInt64(2),

            MachineId =
                reader.GetInt32(3),

            MachineCode =
                reader.GetString(4),

            Result =
                reader.GetString(5),

            SourceProductType =
                reader.IsDBNull(6)
                    ? null
                    : reader.GetString(6),

            InspectedAt =
                new DateTimeOffset(
                    inspectedAtUtc
                ),

            SourceSystem =
                reader.GetString(8),

            ReceivedAt =
                new DateTimeOffset(
                    receivedAtUtc
                )
        };
    }
}