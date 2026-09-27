using Mes.Server.Models.Production;

namespace Mes.Server.Repositories.Production;

public interface IProductionRepository
{
    Task<CurrentProductionInfo?>
        GetCurrentAsync();
}