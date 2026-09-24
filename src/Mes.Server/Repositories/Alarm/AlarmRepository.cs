using System.Data;
using Mes.Server.Database;
using Mes.Server.Models.Alarm;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Repositories.Alarm;

public class AlarmRepository
    : IAlarmRepository
{
    private readonly MesDbConnectionFactory _connectionFactory;

    public AlarmRepository(
        MesDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    // =====================================================
    // 알람 발생 등록
    // =====================================================
    public async Task<AlarmInfo> CreateAsync(
        AlarmCreateRequest request)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            DECLARE @MachineId INT;

            SELECT
                @MachineId = machine_id
            FROM machines
            WHERE machine_code = @MachineCode
              AND is_active = 1;


            IF @MachineId IS NULL
            BEGIN
                THROW 50201, '존재하지 않는 설비입니다.', 1;
            END;


            INSERT INTO alarms
            (
                machine_id,
                alarm_code,
                alarm_message,
                severity,
                occurred_at,
                cleared_at,
                is_active,
                source_system
            )
            VALUES
            (
                @MachineId,
                @AlarmCode,
                @AlarmMessage,
                @Severity,
                @OccurredAt,
                NULL,
                1,
                @SourceSystem
            );


            DECLARE @AlarmId BIGINT =
                SCOPE_IDENTITY();


            SELECT
                a.alarm_id,
                a.machine_id,
                m.machine_code,
                a.alarm_code,
                a.alarm_message,
                a.severity,
                a.occurred_at,
                a.cleared_at,
                a.is_active,
                a.source_system,
                a.received_at

            FROM alarms a

            INNER JOIN machines m
                ON a.machine_id = m.machine_id

            WHERE a.alarm_id = @AlarmId;
            """;


        await using var command =
            new SqlCommand(sql, connection);


        command.Parameters.Add(
            "@MachineCode",
            SqlDbType.VarChar,
            30
        ).Value = request.MachineCode;


        command.Parameters.Add(
            "@AlarmCode",
            SqlDbType.VarChar,
            30
        ).Value = request.AlarmCode;


        command.Parameters.Add(
            "@AlarmMessage",
            SqlDbType.NVarChar,
            200
        ).Value =
            string.IsNullOrWhiteSpace(
                request.AlarmMessage)
                ? DBNull.Value
                : request.AlarmMessage;


        command.Parameters.Add(
            "@Severity",
            SqlDbType.VarChar,
            20
        ).Value = request.Severity;


        command.Parameters.Add(
            "@OccurredAt",
            SqlDbType.DateTime2
        ).Value =
            request.OccurredAt.UtcDateTime;


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
                "생성된 알람 정보를 반환받지 못했습니다."
            );
        }


        return MapAlarm(reader);
    }


    // =====================================================
    // 알람 해제
    //
    // is_active = 0
    // cleared_at 기록
    // =====================================================
    public async Task<AlarmInfo?> ClearAsync(
        long alarmId,
        DateTimeOffset clearedAt)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            UPDATE alarms
            SET
                is_active = 0,
                cleared_at = @ClearedAt
            WHERE alarm_id = @AlarmId
              AND is_active = 1;


            IF @@ROWCOUNT = 0
            BEGIN
                RETURN;
            END;


            SELECT
                a.alarm_id,
                a.machine_id,
                m.machine_code,
                a.alarm_code,
                a.alarm_message,
                a.severity,
                a.occurred_at,
                a.cleared_at,
                a.is_active,
                a.source_system,
                a.received_at

            FROM alarms a

            INNER JOIN machines m
                ON a.machine_id = m.machine_id

            WHERE a.alarm_id = @AlarmId;
            """;


        await using var command =
            new SqlCommand(sql, connection);


        command.Parameters.Add(
            "@AlarmId",
            SqlDbType.BigInt
        ).Value = alarmId;


        command.Parameters.Add(
            "@ClearedAt",
            SqlDbType.DateTime2
        ).Value = clearedAt.UtcDateTime;


        await using var reader =
            await command.ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            return null;
        }


        return MapAlarm(reader);
    }


    // =====================================================
    // 현재 활성 알람 조회
    // =====================================================
    public async Task<IReadOnlyList<AlarmInfo>>
        GetActiveAsync()
    {
        var alarms =
            new List<AlarmInfo>();


        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT
                a.alarm_id,
                a.machine_id,
                m.machine_code,
                a.alarm_code,
                a.alarm_message,
                a.severity,
                a.occurred_at,
                a.cleared_at,
                a.is_active,
                a.source_system,
                a.received_at

            FROM alarms a

            INNER JOIN machines m
                ON a.machine_id = m.machine_id

            WHERE a.is_active = 1

            ORDER BY
                a.occurred_at DESC;
            """;


        await using var command =
            new SqlCommand(sql, connection);


        await using var reader =
            await command.ExecuteReaderAsync();


        while (await reader.ReadAsync())
        {
            alarms.Add(
                MapAlarm(reader)
            );
        }


        return alarms;
    }


    // =====================================================
    // 특정 설비 알람 이력 조회
    // =====================================================
    public async Task<IReadOnlyList<AlarmInfo>>
        GetByMachineCodeAsync(
            string machineCode)
    {
        var alarms =
            new List<AlarmInfo>();


        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            SELECT
                a.alarm_id,
                a.machine_id,
                m.machine_code,
                a.alarm_code,
                a.alarm_message,
                a.severity,
                a.occurred_at,
                a.cleared_at,
                a.is_active,
                a.source_system,
                a.received_at

            FROM alarms a

            INNER JOIN machines m
                ON a.machine_id = m.machine_id

            WHERE m.machine_code = @MachineCode

            ORDER BY
                a.occurred_at DESC;
            """;


        await using var command =
            new SqlCommand(sql, connection);


        command.Parameters.Add(
            "@MachineCode",
            SqlDbType.VarChar,
            30
        ).Value = machineCode;


        await using var reader =
            await command.ExecuteReaderAsync();


        while (await reader.ReadAsync())
        {
            alarms.Add(
                MapAlarm(reader)
            );
        }


        return alarms;
    }


    // =====================================================
    // SqlDataReader → AlarmInfo
    // =====================================================
    private static AlarmInfo MapAlarm(
        SqlDataReader reader)
    {
        var occurredAtUtc =
            DateTime.SpecifyKind(
                reader.GetDateTime(6),
                DateTimeKind.Utc
            );


        DateTimeOffset? clearedAt = null;

        if (!reader.IsDBNull(7))
        {
            var clearedAtUtc =
                DateTime.SpecifyKind(
                    reader.GetDateTime(7),
                    DateTimeKind.Utc
                );

            clearedAt =
                new DateTimeOffset(
                    clearedAtUtc
                );
        }


        var receivedAtUtc =
            DateTime.SpecifyKind(
                reader.GetDateTime(10),
                DateTimeKind.Utc
            );


        return new AlarmInfo
        {
            AlarmId =
                reader.GetInt64(0),

            MachineId =
                reader.GetInt32(1),

            MachineCode =
                reader.GetString(2),

            AlarmCode =
                reader.GetString(3),

            AlarmMessage =
                reader.IsDBNull(4)
                    ? null
                    : reader.GetString(4),

            Severity =
                reader.GetString(5),

            OccurredAt =
                new DateTimeOffset(
                    occurredAtUtc
                ),

            ClearedAt =
                clearedAt,

            IsActive =
                reader.GetBoolean(8),

            SourceSystem =
                reader.GetString(9),

            ReceivedAt =
                new DateTimeOffset(
                    receivedAtUtc
                )
        };
    }
}