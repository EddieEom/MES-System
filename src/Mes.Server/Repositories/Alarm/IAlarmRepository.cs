using Mes.Server.Models.Alarm;

namespace Mes.Server.Repositories.Alarm;

public interface IAlarmRepository
{
    Task<AlarmInfo> CreateAsync(
        AlarmCreateRequest request
    );

    Task<AlarmInfo?> ClearAsync(
        long alarmId,
        DateTimeOffset clearedAt
    );

    Task<IReadOnlyList<AlarmInfo>> GetActiveAsync();

    Task<IReadOnlyList<AlarmInfo>> GetByMachineCodeAsync(
        string machineCode
    );
}