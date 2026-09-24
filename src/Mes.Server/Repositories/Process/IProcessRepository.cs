using Mes.Server.Models.Process;

namespace Mes.Server.Repositories.Process;

public interface IProcessRepository
{
    Task<MoveProductResult> MoveProductAsync(
        MoveProductRequest request
    );

    Task<IReadOnlyList<ProcessEventInfo>>
        GetProductHistoryAsync(
            string productCode
        );
}