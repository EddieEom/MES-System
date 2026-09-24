using Mes.Server.Models.Quality;
using Mes.Server.Repositories.Quality;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Controllers.Quality;

[ApiController]
[Route("api/quality-results")]
public class QualityResultController
    : ControllerBase
{
    private readonly IQualityResultRepository _repository;

    public QualityResultController(
        IQualityResultRepository repository)
    {
        _repository = repository;
    }


    // =====================================================
    // 품질 결과 저장
    //
    // POST /api/quality-results
    // =====================================================
    [HttpPost]
    public async Task<IActionResult> Record(
        [FromBody] QualityResultCreateRequest request)
    {
        try
        {
            var result =
                await _repository.RecordAsync(
                    request
                );

            return Ok(new
            {
                status = "ok",
                production = result
            });
        }

        // Stored Procedure의 THROW 또는 DB 오류
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
    // EventId 기준 품질 결과 조회
    //
    // GET /api/quality-results/{eventId}
    // =====================================================
    [HttpGet("{eventId:guid}")]
    public async Task<IActionResult> Get(
        Guid eventId)
    {
        var result =
            await _repository.GetByEventIdAsync(
                eventId
            );

        if (result is null)
        {
            return NotFound(new
            {
                status = "error",
                message =
                    $"품질 결과를 찾을 수 없습니다: {eventId}"
            });
        }


        return Ok(result);
    }
}