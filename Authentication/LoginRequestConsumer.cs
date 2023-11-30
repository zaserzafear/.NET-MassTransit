using AuthenticationModels;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Authentication
{
    internal class LoginRequestConsumer : IConsumer<LoginRequest>
    {
        private readonly ILogger<LoginRequestConsumer> _logger;

        public LoginRequestConsumer(ILogger<LoginRequestConsumer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<LoginRequest> context)
        {
            try
            {
                var result = new LoginResult
                {
                    IsSuccess = true,
                    Token = "asdf",
                };

                await context.RespondAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing LoginRequest");
                throw;
            }
        }
    }
}
