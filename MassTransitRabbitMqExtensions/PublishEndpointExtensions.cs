using MassTransit;

namespace MassTransitRabbitMqExtensions
{
    public static class PublishEndpointExtensions
    {
        public static async Task PublishMessage<T>(this IPublishEndpoint publishEndpoint, T message)
            where T : class
        {
            await publishEndpoint.Publish<T>(message);
        }
    }
}
