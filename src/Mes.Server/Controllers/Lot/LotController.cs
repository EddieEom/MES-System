using Mes.Server.Repositories.Lot;
using Microsoft.AspNetCore.Mvc;

namespace Mes.Server.Controllers.Lot;

[ApiController]
[Route("api/lots")]
public class LotController
    : ControllerBase
{
    private readonly ILotRepository _repository;

    public LotController(
        ILotRepository repository)
    {
        _repository = repository;
    }


    // =====================================================
    // LOT 단건 조회
    //
    // GET /api/lots/{lotCode}
    // =====================================================
    [HttpGet("{lotCode}")]
    public async Task<IActionResult> Get(
        string lotCode)
    {
        var lot =
            await _repository.GetByCodeAsync(
                lotCode
            );


        if (lot is null)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"LOT을 찾을 수 없습니다: {lotCode}"
            });
        }


        return Ok(lot);
    }


    // =====================================================
    // WorkOrder의 전체 LOT 조회
    //
    // GET /api/lots/work-order/{workOrderCode}
    // =====================================================
    [HttpGet("work-order/{workOrderCode}")]
    public async Task<IActionResult> GetByWorkOrder(
        string workOrderCode)
    {
        var lots =
            await _repository
                .GetByWorkOrderCodeAsync(
                    workOrderCode
                );


        if (lots.Count == 0)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"작업지시의 LOT을 찾을 수 없습니다: {workOrderCode}"
            });
        }


        return Ok(lots);
    }


    // =====================================================
    // 현재 RUNNING LOT 조회
    //
    // GET /api/lots/work-order/{workOrderCode}/running
    // =====================================================
    [HttpGet(
        "work-order/{workOrderCode}/running"
    )]
    public async Task<IActionResult> GetRunning(
        string workOrderCode)
    {
        var lot =
            await _repository.GetRunningAsync(
                workOrderCode
            );


        if (lot is null)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"현재 RUNNING 상태의 LOT이 없습니다: {workOrderCode}"
            });
        }


        return Ok(lot);
    }
}