using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransitRabbitMqExtensions
{
    public static class MassTransitRabbitMqExtensions
    {
        public static IServiceCollection AddMassTransitRabbitMq(this IServiceCollection services, string host, string username, string password)
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
