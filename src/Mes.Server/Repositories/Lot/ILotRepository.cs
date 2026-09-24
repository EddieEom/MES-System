using Mes.Server.Models.Lot;

namespace Mes.Server.Repositories.Lot;

public interface ILotRepository
{
    Task<LotInfo?> GetByCodeAsync(
        string lotCode
    );

    Task<IReadOnlyList<LotInfo>> GetByWorkOrderCodeAsync(
        string workOrderCode
    );

    Task<LotInfo?> GetRunningAsync(
        string workOrderCode
    );
}