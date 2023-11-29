using Microsoft.AspNetCore.Mvc;
using Models;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        [HttpPost]
        public IActionResult Authentication([FromBody] AuthenticationModel model)
        {
            return Ok(model);
        }
    }
}
