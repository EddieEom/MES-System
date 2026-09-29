using Mes.Server.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Controllers;

[ApiController]
[Route("api/database")]
public class DatabaseController : ControllerBase
{
    private readonly MesDbConnectionFactory _connectionFactory;

    public DatabaseController(
        MesDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    // ==========================================
    // Database Health Check
    //
    // 1. 실제 DB 연결 확인
    // 2. MES_SYSTEM 연결 확인
    // 3. 필수 Table 10개 확인
    // 4. Stored Procedure 3개 확인
    // ==========================================
    [HttpGet("health")]
    public async Task<IActionResult> HealthAsync()
    {
        const int expectedTableCount = 10;
        const int expectedProcedureCount = 3;

        try
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();


            const string sql = """
                SELECT
                    DB_NAME() AS database_name,

                    (
                        SELECT COUNT(*)
                        FROM sys.tables
                        WHERE name IN
                        (
                            'users',
                            'work_orders',
                            'lots',
                            'machines',
                            'locations',
                            'products',
                            'process_events',
                            'quality_results',
                            'machine_status_history',
                            'alarms'
                        )
                    ) AS table_count,

                    (
                        SELECT COUNT(*)
                        FROM sys.procedures
                        WHERE name IN
                        (
                            'usp_CreateWorkOrder',
                            'usp_MoveProduct',
                            'usp_RecordQualityResult'
                        )
                    ) AS procedure_count;
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
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new
                    {
                        status = "error",
                        message =
                            "Database Health 조회 결과가 없습니다."
                    }
                );
            }


            string databaseName =
                reader["database_name"]
                    ?.ToString()
                ?? string.Empty;


            int tableCount =
                Convert.ToInt32(
                    reader["table_count"]
                );


            int procedureCount =
                Convert.ToInt32(
                    reader["procedure_count"]
                );


            bool isHealthy =
                databaseName == "MES_SYSTEM"
                && tableCount == expectedTableCount
                && procedureCount == expectedProcedureCount;


            if (!isHealthy)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new
                    {
                        status = "degraded",

                        database =
                            databaseName,

                        tables =
                            $"{tableCount}/{expectedTableCount}",

                        storedProcedures =
                            $"{procedureCount}/{expectedProcedureCount}"
                    }
                );
            }


            return Ok(
                new
                {
                    status = "ok",

                    database =
                        databaseName,

                    tables =
                        $"{tableCount}/{expectedTableCount}",

                    storedProcedures =
                        $"{procedureCount}/{expectedProcedureCount}"
                }
            );
        }
        catch (SqlException ex)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    status = "error",
                    database = "unavailable",
                    message = ex.Message
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    status = "error",
                    message = ex.Message
                }
            );
        }
    }
}