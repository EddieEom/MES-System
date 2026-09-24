using Mes.Server.Models.Machine;
using Mes.Server.Repositories.Machine;
using Microsoft.AspNetCore.Mvc;

namespace Mes.Server.Controllers.Machine;

[ApiController]
[Route("api/machine-status")]
public class MachineStatusController
    : ControllerBase
{
    private readonly IMachineStatusRepository _repository;

    public MachineStatusController(
        IMachineStatusRepository repository)
    {
        _repository = repository;
    }


    // =====================================================
    // 설비 상태 저장
    // POST /api/machine-status
    // =====================================================
    [HttpPost]
    public async Task<IActionResult> Insert(
    [FromBody] MachineStatusCreateRequest request)
    {
        try
        {
            var status = new MachineStatus
            {
                MachineCode = request.MachineCode,
                MeasuredAt = request.MeasuredAt,
                State = request.State,
                PartsEntered = request.PartsEntered,
                PartsExited = request.PartsExited,
                PartsCurrent = request.PartsCurrent,
                PartsAverageTime = request.PartsAverageTime,
                IdlePercentage = request.IdlePercentage,
                BusyPercentage = request.BusyPercentage,
                BlockedPercentage = request.BlockedPercentage,
                FailedPercentage = request.FailedPercentage,
                RepairPercentage = request.RepairPercentage,
                Utilization = request.Utilization,
                SourceSystem = request.SourceSystem
            };

            var id = await _repository.InsertAsync(status);

            return Ok(new
            {
                status = "ok",
                machineStatusHistoryId = id
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                status = "error",
                message = ex.Message
            });
        }
    }


    // =====================================================
    // 최신 설비 상태 조회
    // GET /api/machine-status/{machineCode}/latest
    // =====================================================
    [HttpGet("{machineCode}/latest")]
    public async Task<IActionResult> GetLatest(
        string machineCode)
    {
        var status =
            await _repository.GetLatestAsync(
                machineCode
            );

        if (status is null)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"설비 상태 데이터가 없습니다: {machineCode}"
            });
        }

        return Ok(status);
    }
}