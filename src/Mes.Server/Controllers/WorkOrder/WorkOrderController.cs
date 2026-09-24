using Mes.Server.Models.WorkOrder;
using Mes.Server.Repositories.WorkOrder;
using Mes.Server.Services.WorkOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Controllers.WorkOrder;

[ApiController]
[Route("api/work-orders")]
public class WorkOrderController
    : ControllerBase
{
    private readonly IWorkOrderRepository _repository;
    private readonly IWorkOrderService _service;

    public WorkOrderController(
        IWorkOrderRepository repository, IWorkOrderService service)
    {
        _repository = repository;
        _service = service;
    }


    // =====================================================
    // WorkOrder 생산 시작
    //
    // POST /api/work-orders/{workOrderCode}/start
    // =====================================================
    [HttpPost("{workOrderCode}/start")]
    public async Task<IActionResult> Start(
        string workOrderCode)
    {
        try
        {
            var result =
                await _service.StartAsync(
                    workOrderCode
                );


            return Ok(new
            {
                status = "ok",
                production = result
            });
        }

        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                status = "error",
                message = ex.Message
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
    // WorkOrder 일시정지
    //
    // POST /api/work-orders/{workOrderCode}/pause
    // =====================================================
    [HttpPost("{workOrderCode}/pause")]
    public async Task<IActionResult> Pause(
        string workOrderCode)
    {
        try
        {
            var result =
                await _service.PauseAsync(
                    workOrderCode
                );


            return Ok(new
            {
                status = "ok",
                production = result
            });
        }

        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                status = "error",
                message = ex.Message
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
    // WorkOrder 생산 재개
    //
    // POST /api/work-orders/{workOrderCode}/resume
    // =====================================================
    [HttpPost("{workOrderCode}/resume")]
    public async Task<IActionResult> Resume(
        string workOrderCode)
    {
        try
        {
            var result =
                await _service.ResumeAsync(
                    workOrderCode
                );


            return Ok(new
            {
                status = "ok",
                production = result
            });
        }

        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                status = "error",
                message = ex.Message
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
    // 작업지시 생성
    //
    // POST /api/work-orders
    // =====================================================
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] WorkOrderCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(
            request.WorkOrderCode))
        {
            return BadRequest(new
            {
                status = "error",
                message =
                    "작업지시 코드는 필수입니다."
            });
        }


        try
        {
            var result =
                await _repository.CreateAsync(
                    request
                );


            return Ok(new
            {
                status = "ok",
                workOrder = result
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
    // 작업지시 조회
    //
    // GET /api/work-orders/{workOrderCode}
    // =====================================================
    [HttpGet("{workOrderCode}")]
    public async Task<IActionResult> Get(
        string workOrderCode)
    {
        var workOrder =
            await _repository.GetByCodeAsync(
                workOrderCode
            );


        if (workOrder is null)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"작업지시를 찾을 수 없습니다: {workOrderCode}"
            });
        }


        return Ok(workOrder);
    }
}