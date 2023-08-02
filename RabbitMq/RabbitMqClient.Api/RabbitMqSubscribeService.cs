using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqClient.Api.Domain;
using System.Text;

namespace RabbitMqClient.Api
{
    public class RabbitMqSubscribeService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private IModel _channel;
        private readonly string _nomeDaFila;

        public RabbitMqSubscribeService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new ConnectionFactory()
            {
                HostName = "HostName",
                Port = 0000,
                UserName = "UserName",
                Password = "Password"
            }
             .CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "NameExchange", type: ExchangeType.Fanout, true, false);
            _nomeDaFila = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _nomeDaFila, exchange: "NameExchange", routingKey: "NameQueue");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                ProcessEvent(message);
            };

            _channel.BasicConsume("NomeDaFila", true, consumer: consumer);

            return Task.CompletedTask;
        }

        private static void ProcessEvent(string message)
        {
            var model = JsonConvert.DeserializeObject<ModelDto>(message);

            Console.WriteLine(model.Name);
            Console.WriteLine(model.Idade);
        }
    }
}