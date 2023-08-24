using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMqClient.Api.Domain;
using RabbitMqClient.Api.Events.Notifications;
using System.Text;
using System.Text.Json;

namespace RabbitMqClient.Api.Services
{
    public class RabbitMqClientService : IRabbitMqClientService
    {
        private readonly ILogger<RabbitMqClientService> _logger;
        private readonly ConnectionRabbit _configs;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqClientService(ILogger<RabbitMqClientService> logger, IOptions<ConnectionRabbit> configs)
        {
            _logger = logger;
            _configs = configs.Value;
            try
            {
                _connection = new ConnectionFactory()
                {
                    HostName = _configs.Host,
                    Port = _configs.Port,
                    UserName = _configs.UserName,
                    VirtualHost = _configs.Environment,
                    Password = _configs.Password,
                }
                .CreateConnection();

                _channel = _connection.CreateModel();
                ConfigureWay();
            }
            catch (Exception)
            {
                _logger.LogError(600, ConstantsRabbit.ErrorRabbit, string.Format(RabbitResources.ErrorConnectionDescription, _configs.Host, _configs.Port));
            }
        }

        public void PublishMessage(EventNotification model, CancellationToken cancellationToken)
        {
            try
            {
                string mensagem = JsonSerializer.Serialize(model);
                var body = Encoding.UTF8.GetBytes(mensagem);

                IBasicProperties basicProperties = _channel.CreateBasicProperties();
                basicProperties.DeliveryMode = 2;

                _channel.BasicPublish(exchange: ConstantsRabbit.NameExchange,
                    routingKey: ConstantsRabbit.NameQueue,
                    basicProperties: basicProperties,
                    body: body);
            }
            catch (Exception ex)
            {
                _logger.LogError(601, ConstantsRabbit.ErrorRabbit, string.Format(RabbitResources.ErrorGenerated, ex));
                throw;
            }
        }

        #region Private Methods

        private void ConfigureWay()
        {
            var arguments = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", ConstantsRabbit.NameExchangeDeadLetter},
            };

            _channel.ExchangeDeclare(exchange: ConstantsRabbit.NameExchange, type: ExchangeType.Fanout, durable: true, autoDelete: false);
            _channel.QueueDeclare(queue: ConstantsRabbit.NameQueue, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
            _channel.QueueBind(queue: ConstantsRabbit.NameQueue, exchange: ConstantsRabbit.NameExchange, routingKey: string.Empty);
        }

        #endregion
    }
}