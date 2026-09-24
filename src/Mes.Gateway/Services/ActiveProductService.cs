using Mes.Gateway.Models;
using Mes.Gateway.Models.MoveProduct;

namespace Mes.Gateway.Services;

public class ActiveProductService
{
    private ProductApiResponse? _currentProduct;


    public ProductApiResponse? CurrentProduct =>
        _currentProduct;


    public bool HasActiveProduct =>
        _currentProduct != null;


    public void Set(
        ProductApiResponse product)
    {
        _currentProduct =
            product;

        Console.WriteLine(
            $"[PRODUCT CONTEXT] 활성 Product 설정 - {product.ProductCode} / ID={product.ProductId}"
        );
    }


    public void Clear()
    {
        if (_currentProduct != null)
        {
            Console.WriteLine(
                $"[PRODUCT CONTEXT] 활성 Product 해제 - {_currentProduct.ProductCode}"
            );
        }

        _currentProduct =
            null;
    }
}