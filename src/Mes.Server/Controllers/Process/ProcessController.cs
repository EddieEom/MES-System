using Mes.Server.Models.Process;
using Mes.Server.Repositories.Process;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Controllers.Process;

[ApiController]
[Route("api/process")]
public class ProcessController
    : ControllerBase
{
    private readonly IProcessRepository _repository;

    public ProcessController(
        IProcessRepository repository)
    {
        _repository = repository;
    }


    // =====================================================
    // 제품 위치 / 공정 이동
    //
    // POST /api/process/move
    // =====================================================
    [HttpPost("move")]
    public async Task<IActionResult> Move(
        [FromBody] MoveProductRequest request)
    {
        if (request.ProductId <= 0)
        {
            return BadRequest(new
            {
                status = "error",
                message = "ProductId가 올바르지 않습니다."
            });
        }


        if (string.IsNullOrWhiteSpace(
            request.ToLocationCode))
        {
            return BadRequest(new
            {
                status = "error",
                message = "ToLocationCode는 필수입니다."
            });
        }


        try
        {
            var result =
                await _repository.MoveProductAsync(
                    request
                );


            return Ok(new
            {
                status = "ok",
                product = result
            });
        }

        catch (SqlException ex)
        {
            return BadRequest(new
            {
                status = "error",
                message = ex.Message
            });
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
    // Product 공정 이력
    //
    // GET /api/process/products/{productCode}/history
    // =====================================================
    [HttpGet(
        "products/{productCode}/history"
    )]
    public async Task<IActionResult> GetHistory(
        string productCode)
    {
        var events =
            await _repository
                .GetProductHistoryAsync(
                    productCode
                );


        if (events.Count == 0)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"제품의 공정 이력이 없습니다: {productCode}"
            });
        }


        return Ok(events);
    }

    // =====================================================
    // 최근 공정 이벤트 조회
    //
    // HMI 대시보드용
    // 기본 10건 / 최대 100건
    // =====================================================
    [HttpGet("recent")]
    public async Task<ActionResult<IReadOnlyList<ProcessEventInfo>>>
        GetRecentEvents(
            [FromQuery] int count = 10)
    {
        if (count < 1 || count > 100)
        {
            return BadRequest(
                new
                {
                    message =
                        "count는 1 이상 100 이하이어야 합니다."
                }
            );
        }

        var events =
            await _repository
                .GetRecentEventsAsync(count);

        return Ok(events);
    }
}