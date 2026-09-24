using System.Data;
using Mes.Server.Database;
using Mes.Server.Models.Process;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Repositories.Process;

public class ProcessRepository
    : IProcessRepository
{
    private readonly MesDbConnectionFactory _connectionFactory;

    public ProcessRepository(
        MesDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    // =====================================================
    // 제품 공정 이동
    //
    // usp_MoveProduct 호출
    //
    // 저장 프로시저 내부에서:
    //
    // 1. Product 검증
    // 2. 목적지 Location 검증
    // 3. Machine 검증
    // 4. 기존 위치 확인
    // 5. products 현재 위치 / 상태 변경
    // 6. process_events 이력 INSERT
    // 7. 최종 위치라면 COMPLETED 처리
    // 8. PASS / FAIL 최종 경로 검증
    //
    // 모두 하나의 Transaction으로 처리
    // =====================================================
    public async Task<MoveProductResult> MoveProductAsync(
        MoveProductRequest request)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        await using var command =
            new SqlCommand(
                "dbo.usp_MoveProduct",
                connection
            );

        command.CommandType =
            CommandType.StoredProcedure;

        command.Parameters.Add(
            "@EventId",
            SqlDbType.UniqueIdentifier
        ).Value = request.EventId;

        command.Parameters.Add(
            "@ProductId",
            SqlDbType.BigInt
        ).Value = request.ProductId;


        command.Parameters.Add(
            "@ToLocationCode",
            SqlDbType.VarChar,
            30
        ).Value = request.ToLocationCode;


        command.Parameters.Add(
            "@EventType",
            SqlDbType.VarChar,
            30
        ).Value = request.EventType;

        command.Parameters.Add(
            "@EventTime",
            SqlDbType.DateTime2
        ).Value =
            request.EventTime.HasValue
                ? request.EventTime
                    .Value
                    .UtcDateTime
                : DBNull.Value;


        command.Parameters.Add(
            "@SourceSystem",
            SqlDbType.VarChar,
            20
        ).Value = request.SourceSystem;


        command.Parameters.Add(
            "@Remarks",
            SqlDbType.NVarChar,
            200
        ).Value =
            string.IsNullOrWhiteSpace(
                request.Remarks)
                ? DBNull.Value
                : request.Remarks;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException(
                "제품 이동 처리 결과를 반환받지 못했습니다."
            );
        }


        return new MoveProductResult
        {
            ProductId =
                Convert.ToInt64(
                    reader["product_id"]
                ),

            ProductCode =
                reader["product_code"]
                    .ToString()
                ?? string.Empty,

            Status =
                reader["status"]
                    .ToString()
                ?? string.Empty,

            QualityStatus =
                reader["quality_status"]
                    .ToString()
                ?? string.Empty,

            CurrentLocation =
                reader["current_location"]
                    .ToString()
                ?? string.Empty
        };
    }


    // =====================================================
    // 제품 전체 공정 이력 조회
    //
    // Product Traceability의 핵심 조회
    //
    // 시간순으로 제품이 어디서 어디로 이동했는지 확인
    // =====================================================
    public async Task<IReadOnlyList<ProcessEventInfo>>
        GetProductHistoryAsync(
            string productCode)
    {
        var events =
            new List<ProcessEventInfo>();


        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT
                pe.process_event_id,
                pe.event_id,
                pe.product_id,
                p.product_code,

                pe.machine_id,
                m.machine_code,

                pe.event_type,

                pe.from_location_id,
                from_loc.location_code
                    AS from_location_code,

                pe.to_location_id,
                to_loc.location_code
                    AS to_location_code,

                pe.event_time,
                pe.source_system,
                pe.remarks,
                pe.created_at

            FROM process_events pe

            INNER JOIN products p
                ON pe.product_id = p.product_id

            LEFT JOIN machines m
                ON pe.machine_id = m.machine_id

            LEFT JOIN locations from_loc
                ON pe.from_location_id =
                   from_loc.location_id

            LEFT JOIN locations to_loc
                ON pe.to_location_id =
                   to_loc.location_id

            WHERE p.product_code = @ProductCode

            ORDER BY
                pe.event_time,
                pe.process_event_id;
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


        while (await reader.ReadAsync())
        {
            var eventTimeUtc =
                DateTime.SpecifyKind(
                    reader.GetDateTime(11),
                    DateTimeKind.Utc
                );

            var createdAtUtc =
                DateTime.SpecifyKind(
                    reader.GetDateTime(14),
                    DateTimeKind.Utc
                );


            events.Add(
                new ProcessEventInfo
                {
                    ProcessEventId =
                        reader.GetInt64(0),

                    EventId =
                        reader.GetGuid(1),

                    ProductId =
                        reader.GetInt64(2),

                    ProductCode =
                        reader.GetString(3),

                    MachineId =
                        reader.IsDBNull(4)
                            ? null
                            : reader.GetInt32(4),

                    MachineCode =
                        reader.IsDBNull(5)
                            ? null
                            : reader.GetString(5),

                    EventType =
                        reader.GetString(6),

                    FromLocationId =
                        reader.IsDBNull(7)
                            ? null
                            : reader.GetInt32(7),

                    FromLocationCode =
                        reader.IsDBNull(8)
                            ? null
                            : reader.GetString(8),

                    ToLocationId =
                        reader.IsDBNull(9)
                            ? null
                            : reader.GetInt32(9),

                    ToLocationCode =
                        reader.IsDBNull(10)
                            ? null
                            : reader.GetString(10),

                    EventTime =
                        new DateTimeOffset(
                            eventTimeUtc
                        ),

                    SourceSystem =
                        reader.GetString(12),

                    Remarks =
                        reader.IsDBNull(13)
                            ? null
                            : reader.GetString(13),

                    CreatedAt =
                        new DateTimeOffset(
                            createdAtUtc
                        )
                }
            );
        }


        return events;
    }
}