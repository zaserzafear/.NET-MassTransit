using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("Readiness")]
        public IActionResult Readiness()
        {
            return Ok();
        }

        [HttpGet("Liveness ")]
        public IActionResult Liveness()
        {
            return Ok();
        }
    }
}
