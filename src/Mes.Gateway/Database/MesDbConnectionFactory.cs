using Microsoft.Data.SqlClient;

namespace Mes.Gateway.Database;

public class MesDbConnectionFactory
{
    private readonly string _connectionString;

    public MesDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public async Task TestConnectionAsync()
    {
        await using var connection = CreateConnection();

        await connection.OpenAsync();

        Console.WriteLine("[DB] MSSQL 연결 성공");

        // 현재 연결된 DB 확인
        await using var databaseCommand = connection.CreateCommand();

        databaseCommand.CommandText = "SELECT DB_NAME();";

        var databaseName =
            await databaseCommand.ExecuteScalarAsync();

        Console.WriteLine(
            $"[DB] Database : {databaseName}"
        );


        // MES 핵심 테이블 존재 여부 확인
        await using var tableCommand = connection.CreateCommand();

        tableCommand.CommandText = """
            SELECT COUNT(*)
            FROM sys.tables
            WHERE name IN
            (
                'work_orders',
                'lots',
                'products',
                'machines',
                'locations',
                'process_events',
                'quality_results',
                'machine_status_history',
                'alarms',
                'users'
            );
            """;

        var tableCount =
            Convert.ToInt32(
                await tableCommand.ExecuteScalarAsync()
            );

        Console.WriteLine(
            $"[DB] MES Table : {tableCount}/10"
        );


        // Stored Procedure 존재 여부 확인
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

        Console.WriteLine(
            $"[DB] Stored Procedure : {procedureCount}/3"
        );
    }
}