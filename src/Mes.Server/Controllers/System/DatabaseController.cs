using Mes.Server.Database;
using Microsoft.AspNetCore.Mvc;

namespace Mes.Server.Controllers.System;

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


    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        try
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();


            // 현재 DB 확인
            await using var dbCommand =
                connection.CreateCommand();

            dbCommand.CommandText =
                "SELECT DB_NAME();";

            var databaseName =
                await dbCommand.ExecuteScalarAsync();


            // MES Table 확인
            await using var tableCommand =
                connection.CreateCommand();

            tableCommand.CommandText = """
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
                );
                """;

            var tableCount =
                Convert.ToInt32(
                    await tableCommand.ExecuteScalarAsync()
                );


            // Stored Procedure 확인
            await using var procedureCommand =
                connection.CreateCommand();

            procedureCommand.CommandText = """
                SELECT COUNT(*)
                FROM sys.procedures
                WHERE name IN
                (
                    'usp_CreateWorkOrder',
                    'usp_MoveProduct',
                    'usp_RecordQualityResult'
                );
                """;

            var procedureCount =
                Convert.ToInt32(
                    await procedureCommand.ExecuteScalarAsync()
                );


            return Ok(new
            {
                status = "ok",
                database = databaseName,
                tables = $"{tableCount}/10",
                storedProcedures = $"{procedureCount}/3"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    status = "error",
                    message = ex.Message
                }
            );
        }
    }
}