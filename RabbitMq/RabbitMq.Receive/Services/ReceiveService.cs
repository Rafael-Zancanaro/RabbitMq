using Newtonsoft.Json;
using RabbitMqClient.Receive.Domain;
using RabbitMqClient.Send.Events.Notifications;

namespace RabbitMqClient.Receive.Services
{
    public class ReceiveService : IReceiveService
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
