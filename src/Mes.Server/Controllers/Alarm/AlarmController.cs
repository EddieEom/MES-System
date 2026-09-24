using Mes.Server.Models.Alarm;
using Mes.Server.Repositories.Alarm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Controllers.Alarm;

[ApiController]
[Route("api/alarms")]
public class AlarmController
    : ControllerBase
{
    private readonly IAlarmRepository _repository;

    public AlarmController(
        IAlarmRepository repository)
    {
        _repository = repository;
    }


    // =====================================================
    // 알람 발생
    //
    // POST /api/alarms
    // =====================================================
    [HttpPost]
    public async Task<IActionResult> Create(
    [FromBody] AlarmCreateRequest request)
    {
        // MachineCode 필수
        if (string.IsNullOrWhiteSpace(
            request.MachineCode))
        {
            return BadRequest(new
            {
                status = "error",
                message = "MachineCode는 필수입니다."
            });
        }


        // AlarmCode 필수
        if (string.IsNullOrWhiteSpace(
            request.AlarmCode))
        {
            return BadRequest(new
            {
                status = "error",
                message = "AlarmCode는 필수입니다."
            });
        }


        // Severity 검증
        if (request.Severity is not
            ("INFO" or "WARNING" or "CRITICAL"))
        {
            return BadRequest(new
            {
                status = "error",
                message =
                    "Severity는 INFO, WARNING, CRITICAL 중 하나여야 합니다."
            });
        }


        // SourceSystem 검증
        if (request.SourceSystem is not
            ("GEMINI" or "PLC"))
        {
            return BadRequest(new
            {
                status = "error",
                message =
                    "SourceSystem은 GEMINI 또는 PLC여야 합니다."
            });
        }


        try
        {
            var alarm =
                await _repository.CreateAsync(
                    request
                );


            return Ok(new
            {
                status = "ok",
                alarm
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
    // 활성 알람 전체 조회
    //
    // GET /api/alarms/active
    // =====================================================
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var alarms =
            await _repository.GetActiveAsync();

        return Ok(alarms);
    }


    // =====================================================
    // 설비별 알람 이력
    //
    // GET /api/alarms/machine/{machineCode}
    // =====================================================
    [HttpGet("machine/{machineCode}")]
    public async Task<IActionResult> GetByMachine(
        string machineCode)
    {
        var alarms =
            await _repository
                .GetByMachineCodeAsync(
                    machineCode
                );


        return Ok(alarms);
    }


    // =====================================================
    // 알람 해제
    //
    // POST /api/alarms/{alarmId}/clear
    // =====================================================
    [HttpPost("{alarmId:long}/clear")]
    public async Task<IActionResult> Clear(
        long alarmId)
    {
        var alarm =
            await _repository.ClearAsync(
                alarmId,
                DateTimeOffset.UtcNow
            );


        if (alarm is null)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"활성 상태의 알람을 찾을 수 없습니다: {alarmId}"
            });
        }


        return Ok(new
        {
            status = "ok",
            alarm
        });
    }
}