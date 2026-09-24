using Mes.Server.Repositories.Product;
using Microsoft.AspNetCore.Mvc;

namespace Mes.Server.Controllers.Product;

[ApiController]
[Route("api/products")]
public class ProductController
    : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductController(
        IProductRepository repository)
    {
        _repository = repository;
    }


    // =====================================================
    // 현재 공정 진행 중 Product 조회
    //
    // GET /api/products/process/current
    // =====================================================
    [HttpGet("process/current")]
    public async Task<IActionResult> GetCurrentInProcess()
    {
        try
        {
            var product =
                await _repository
                    .GetCurrentInProcessAsync();


            if (product is null)
            {
                return NotFound(new
                {
                    status = "error",
                    message =
                        "현재 IN_PROCESS Product가 없습니다."
                });
            }


            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    status = "error",
                    message = ex.Message
                }
            );
        }
    }


    // =====================================================
    // 현재 품질판정 대상 Product 조회
    //
    // GET /api/products/quality/current
    // =====================================================
    [HttpGet("quality/current")]
    public async Task<IActionResult> GetCurrentQualityPending()
    {
        try
        {
            var product =
                await _repository
                    .GetCurrentQualityPendingAsync();


            if (product is null)
            {
                return NotFound(new
                {
                    status = "error",
                    message =
                        "현재 품질판정 대기 중인 Product가 없습니다."
                });
            }


            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    status = "error",
                    message = ex.Message
                }
            );
        }
    }

    // =====================================================
    // Product 단건 조회
    //
    // GET /api/products/{productCode}
    // =====================================================
    [HttpGet("{productCode}")]
    public async Task<IActionResult> Get(
        string productCode)
    {
        var product =
            await _repository.GetByCodeAsync(
                productCode
            );


        if (product is null)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"제품을 찾을 수 없습니다: {productCode}"
            });
        }


        return Ok(product);
    }


    // =====================================================
    // LOT의 전체 Product 조회
    //
    // GET /api/products/lot/{lotCode}
    // =====================================================
    [HttpGet("lot/{lotCode}")]
    public async Task<IActionResult> GetByLot(
        string lotCode)
    {
        var products =
            await _repository.GetByLotCodeAsync(
                lotCode
            );


        if (products.Count == 0)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"LOT의 제품을 찾을 수 없습니다: {lotCode}"
            });
        }


        return Ok(products);
    }


    // =====================================================
    // LOT의 다음 생산 대상 Product 조회
    //
    // GET /api/products/lot/{lotCode}/next-waiting
    // =====================================================
    [HttpGet(
        "lot/{lotCode}/next-waiting"
    )]
    public async Task<IActionResult> GetNextWaiting(
        string lotCode)
    {
        var product =
            await _repository.GetNextWaitingAsync(
                lotCode
            );


        if (product is null)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"다음 WAITING 제품이 없습니다: {lotCode}"
            });
        }


        return Ok(product);
    }
}