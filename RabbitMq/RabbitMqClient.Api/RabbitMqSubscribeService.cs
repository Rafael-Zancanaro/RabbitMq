using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqClient.Api.Domain;
using System.Text;

namespace RabbitMqClient.Api
{
    public class RabbitMqSubscribeService : BackgroundService
    {
        private readonly ILogger<RabbitMqSubscribeService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string _nomeDaFila;

        public RabbitMqSubscribeService(ILogger<RabbitMqSubscribeService> logger)
        {
            _logger = logger;

            try
            {
                _connection = new ConnectionFactory()
                {
                HostName = "HostName",
                Port = 0000,
                UserName = "UserName",
                Password = "Password"
                }
             .CreateConnection();

                _channel = _connection.CreateModel();
                ConfigureWay();
            }
            catch (Exception)
            {
                _logger.LogError(504, "Error To Connect RabbitMq", $"Host: {"localhost"}, Port: {5672}");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ModuleHandle, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());

                    ProcessEvent(message);
                };

                _channel.BasicConsume("NameQueue", false, consumer);
                _channel.BasicAck(1, false);
            }
            catch (Exception)
            {
                _channel.BasicNack(1, false, true);
            }

            return Task.CompletedTask;
        }

        private static void ProcessEvent(string message)
        {
            var model = JsonConvert.DeserializeObject<ModelDto>(message);

            Console.WriteLine(model.Name);
            Console.WriteLine(model.Idade);
        }

        #region Private Methods

        private void ConfigureWay()
        {
            _channel.ExchangeDeclare(exchange: "NameExchange", type: ExchangeType.Fanout, true, false);
            _nomeDaFila = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _nomeDaFila, exchange: "NameExchange", routingKey: "NameQueue");
        }

        #endregion
    }
}