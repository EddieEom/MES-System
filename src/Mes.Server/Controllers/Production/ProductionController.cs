using Mes.Server.Repositories.Production;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Controllers.Production;

[ApiController]
[Route("api/production")]
public class ProductionController
    : ControllerBase
{
    private readonly IProductionRepository
        _repository;


    public ProductionController(
        IProductionRepository repository)
    {
        _repository =
            repository;
    }


    // ==============================================
    // 현재 생산실적
    //
    // GET /api/production/current
    // ==============================================

    [HttpGet("current")]
    public async Task<IActionResult>
        GetCurrent()
    {
        try
        {
            var result =
                await _repository
                    .GetCurrentAsync();


            if (result is null)
            {
                return NotFound(
                    new
                    {
                        status = "error",

                        message =
                            "현재 활성 WorkOrder가 없습니다."
                    }
                );
            }


            return Ok(result);
        }
        catch (SqlException ex)
        {
            return BadRequest(
                new
                {
                    status = "error",
                    message = ex.Message
                }
            );
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
}