using Microsoft.Data.SqlClient;

namespace Mes.Server.Database;

public class MesDbConnectionFactory
{
    private readonly string _connectionString;

    public MesDbConnectionFactory(
        IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("MesDatabase")
            ?? throw new InvalidOperationException(
                "MES Database ConnectionString이 설정되어 있지 않습니다."
            );
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}