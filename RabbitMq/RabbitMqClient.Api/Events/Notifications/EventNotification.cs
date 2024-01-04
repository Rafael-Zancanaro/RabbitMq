using MediatR;

namespace RabbitMqClient.Send.Events.Notifications
{
    public class EventNotification : INotification
    {
        public int TypeEvent { get; set; }
        public string User { get; set; }
        public DateTime DateEvent { get; set; }
    }
}