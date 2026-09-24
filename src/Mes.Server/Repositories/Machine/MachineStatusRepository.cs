using Mes.Server.Database;
using Mes.Server.Models.Machine;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Repositories.Machine;

public class MachineStatusRepository
    : IMachineStatusRepository
{
    private readonly MesDbConnectionFactory _connectionFactory;

    public MachineStatusRepository(
        MesDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    // =====================================================
    // 설비 상태 이력 저장
    // =====================================================
    public async Task<long> InsertAsync(
        MachineStatus status)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            INSERT INTO machine_status_history
            (
                machine_id,
                measured_at,
                state,
                parts_entered,
                parts_exited,
                parts_current,
                parts_average_time,
                idle_percentage,
                busy_percentage,
                blocked_percentage,
                failed_percentage,
                repair_percentage,
                utilization,
                source_system
            )
            OUTPUT INSERTED.machine_status_history_id
            SELECT
                m.machine_id,
                @MeasuredAt,
                @State,
                @PartsEntered,
                @PartsExited,
                @PartsCurrent,
                @PartsAverageTime,
                @IdlePercentage,
                @BusyPercentage,
                @BlockedPercentage,
                @FailedPercentage,
                @RepairPercentage,
                @Utilization,
                @SourceSystem
            FROM machines m
            WHERE m.machine_code = @MachineCode
              AND m.is_active = 1;
            """;


        await using var command =
            new SqlCommand(sql, connection);


        command.Parameters.AddWithValue(
            "@MachineCode",
            status.MachineCode
        );

        command.Parameters.AddWithValue(
            "@MeasuredAt",
            status.MeasuredAt.UtcDateTime
        );

        command.Parameters.AddWithValue(
            "@State",
            status.State
        );

        command.Parameters.AddWithValue(
            "@PartsEntered",
            status.PartsEntered
        );

        command.Parameters.AddWithValue(
            "@PartsExited",
            status.PartsExited
        );

        command.Parameters.AddWithValue(
            "@PartsCurrent",
            status.PartsCurrent
        );

        command.Parameters.AddWithValue(
            "@PartsAverageTime",
            status.PartsAverageTime
        );

        command.Parameters.AddWithValue(
            "@IdlePercentage",
            status.IdlePercentage
        );

        command.Parameters.AddWithValue(
            "@BusyPercentage",
            status.BusyPercentage
        );

        command.Parameters.AddWithValue(
            "@BlockedPercentage",
            status.BlockedPercentage
        );

        command.Parameters.AddWithValue(
            "@FailedPercentage",
            status.FailedPercentage
        );

        command.Parameters.AddWithValue(
            "@RepairPercentage",
            status.RepairPercentage
        );

        command.Parameters.AddWithValue(
            "@Utilization",
            status.Utilization
        );

        command.Parameters.AddWithValue(
            "@SourceSystem",
            status.SourceSystem
        );


        var result =
            await command.ExecuteScalarAsync();


        if (result is null)
        {
            throw new InvalidOperationException(
                $"등록되지 않은 설비 코드입니다: {status.MachineCode}"
            );
        }


        return Convert.ToInt64(result);
    }


    // =====================================================
    // 특정 설비의 최신 상태 조회
    // =====================================================
    public async Task<MachineStatus?> GetLatestAsync(
        string machineCode)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT TOP 1
                msh.machine_status_history_id,
                msh.machine_id,
                m.machine_code,
                msh.measured_at,
                msh.state,
                msh.parts_entered,
                msh.parts_exited,
                msh.parts_current,
                msh.parts_average_time,
                msh.idle_percentage,
                msh.busy_percentage,
                msh.blocked_percentage,
                msh.failed_percentage,
                msh.repair_percentage,
                msh.utilization,
                msh.source_system,
                msh.received_at

            FROM machine_status_history msh

            INNER JOIN machines m
                ON msh.machine_id = m.machine_id

            WHERE m.machine_code = @MachineCode

            ORDER BY msh.measured_at DESC;
            """;


        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@MachineCode",
            machineCode
        );


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            return null;
        }


        return new MachineStatus
        {
            MachineStatusHistoryId =
                reader.GetInt64(0),

            MachineId =
                reader.GetInt32(1),

            MachineCode =
                reader.GetString(2),

            MeasuredAt =
                reader.GetDateTime(3),

            State =
                reader.GetString(4),

            PartsEntered =
                reader.GetInt32(5),

            PartsExited =
                reader.GetInt32(6),

            PartsCurrent =
                reader.GetInt32(7),

            PartsAverageTime =
                reader.GetDecimal(8),

            IdlePercentage =
                reader.GetDecimal(9),

            BusyPercentage =
                reader.GetDecimal(10),

            BlockedPercentage =
                reader.GetDecimal(11),

            FailedPercentage =
                reader.GetDecimal(12),

            RepairPercentage =
                reader.GetDecimal(13),

            Utilization =
                reader.GetDecimal(14),

            SourceSystem =
                reader.GetString(15),

            ReceivedAt =
                reader.GetDateTime(16)
        };
    }
}