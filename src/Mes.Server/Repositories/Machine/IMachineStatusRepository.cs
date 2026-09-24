using Mes.Server.Models.Machine;

namespace Mes.Server.Repositories.Machine;

public interface IMachineStatusRepository
{
    // INSERT
    Task<long> InsertAsync(
        MachineStatus status
    );

    // 최신 상태 Select

    Task<MachineStatus?> GetLatestAsync(
        string machineCode
    );
}