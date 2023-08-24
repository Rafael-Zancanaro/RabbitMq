using RabbitMqClient.Api.Events.Notifications;

namespace RabbitMqClient.Api.Domain
{
    public interface IRabbitMqClientService
    {
        void PublishMessage(EventNotification model, CancellationToken cancellationToken);
    }
}