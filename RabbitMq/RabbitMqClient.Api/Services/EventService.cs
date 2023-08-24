using Newtonsoft.Json;
using RabbitMqClient.Api.Domain;
using RabbitMqClient.Api.Events.Notifications;

namespace RabbitMqClient.Api.Services
{
    public class EventService : IEventService
    {
        public Task ProcessEventAsync(string message)
        {
            //Do something
            var model = JsonConvert.DeserializeObject<EventNotification>(message) ?? throw new Exception(message);

            Console.WriteLine(model.TypeEvent);
            Console.WriteLine(model.User);

            return Task.CompletedTask;
        }
    }
}
