using RabbitMqClient.Send.Events.Notifications;

namespace RabbitMqClient.Send.Domain
{
    public interface ISendService
    {
        void PublishMessage(EventNotification model, CancellationToken cancellationToken);
    }
}