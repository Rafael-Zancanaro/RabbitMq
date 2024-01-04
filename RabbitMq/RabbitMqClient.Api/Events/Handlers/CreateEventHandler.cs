using MediatR;
using RabbitMqClient.Send.Domain;
using RabbitMqClient.Send.Events.Notifications;

namespace RabbitMqClient.Send.Events.Handlers
{
    public class CreateEventHandler : INotificationHandler<EventNotification>
    {
        private readonly ISendService _sendService;

        public CreateEventHandler(ISendService rabbitMqClientService)
        {
            _sendService = rabbitMqClientService;
        }

        public Task Handle(EventNotification notification, CancellationToken cancellationToken)
        {
            if (notification is not null)
                _sendService.PublishMessage(notification, cancellationToken);

            return Task.CompletedTask;
        }
    }
}
