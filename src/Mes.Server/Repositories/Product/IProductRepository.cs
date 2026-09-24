using Mes.Server.Models.Product;

namespace Mes.Server.Repositories.Product;

public interface IProductRepository
{
    Task<ProductInfo?> GetByCodeAsync(
        string productCode
    );

    Task<IReadOnlyList<ProductInfo>> GetByLotCodeAsync(
        string lotCode
    );

    Task<ProductInfo?> GetNextWaitingAsync(
        string lotCode
    );
}