using MediatR;
using RabbitMqClient.Api.Domain;
using RabbitMqClient.Api.Events.Notifications;

namespace RabbitMqClient.Api.Events.Handlers
{
    public class CreateEventHandler : INotificationHandler<EventNotification>
    {
        private readonly IRabbitMqClientService _rabbitMqClientService;

        public CreateEventHandler(IRabbitMqClientService rabbitMqClientService)
        {
            _rabbitMqClientService = rabbitMqClientService;
        }

        public Task Handle(EventNotification notification, CancellationToken cancellationToken)
        {
            if (notification is not null)
                _rabbitMqClientService.PublishMessage(notification, cancellationToken);

            return Task.CompletedTask;
        }
    }
}
