using RabbitMQ.Client;
using RabbitMqClient.Api.Domain;
using System.Text;
using System.Text.Json;

namespace RabbitMqClient.Api.Services
{
    public class RabbitMqClientService : IRabbitMqClientService
    {
        private readonly ILogger<RabbitMqClientService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqClientService(ILogger<RabbitMqClientService> logger)
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
                _logger.LogError(600, "Error To Connect RabbitMq", $"Host: {"localhost"}, Port: {5672}");
            }
        }

        public void PublishMessage(ModelDto model)
        {
            try
            {
                string mensagem = JsonSerializer.Serialize(model);
                var body = Encoding.UTF8.GetBytes(mensagem);

                IBasicProperties basicProperties;
                //basicProperties.DeliveryMode = 2;

                _channel.BasicPublish(exchange: "NameExchange",
                    routingKey: "NameQueue",
                    basicProperties: null,//basicProperties,
                    body: body);
            }
            catch (Exception ex)
            {
                _logger.LogError(601, "Error To Connect RabbitMq", $"| exception generated: {ex.Message} |");
                throw;
            }
        }

        #region Private Methods

        private void ConfigureWay()
        {
            _channel.ExchangeDeclare(exchange: "NameExchange", type: ExchangeType.Fanout, durable: true, autoDelete: false);
            _channel.QueueDeclare(queue: "NameQueue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: "NameQueue", exchange: "NameExchange", routingKey: "NameQueue");
        }

        #endregion
    }
}