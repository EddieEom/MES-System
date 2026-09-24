using Microsoft.AspNetCore.Mvc;

namespace Mes.Server.Controllers.System
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "ok",
                service = "MES Server",
                timestamp = DateTime.UtcNow
            });
        }
    }
}