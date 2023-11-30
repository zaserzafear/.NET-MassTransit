using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Extensions
{
    internal static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitExtension(this IServiceCollection services, string host, string username, string password)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri($"rabbitmq://{host}"), h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    cfg.ConfigureEndpoints(context);
                });

                x.AddConsumer<LoginRequestConsumer>();
            });

            return services;
        }

        public static async Task<IServiceCollection> StartMassTransitAsync(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var busControl = serviceProvider.GetRequiredService<IBusControl>();
            await busControl.StartAsync();
            return services;
        }
    }
}
