using AuthenticationModels;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IRequestClient<LoginRequest> _requestClient;

        public AuthenticationController(IRequestClient<LoginRequest> requestClient)
        {
            _requestClient = requestClient;
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticationAsync([FromBody] LoginRequest model)
        {
            var response = await _requestClient.GetResponse<LoginResult>(model);

            var result = response.Message;

            return Ok(result);
        }
    }
}
