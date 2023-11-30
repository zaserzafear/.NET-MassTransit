using MassTransit;

namespace Api.Extensions
{
    internal static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitExtension(this IServiceCollection services, string host, string username, string password)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    // Configure RabbitMQ host, username, and password
                    cfg.Host(new Uri($"rabbitmq://{host}"), h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });
                });
            });

            return services;
        }
    }
}
