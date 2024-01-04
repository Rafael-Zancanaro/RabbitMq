using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMqClient.Send.Domain;
using RabbitMqClient.Send.Events.Notifications;
using System.Text;
using System.Text.Json;

namespace RabbitMqClient.Send.Services
{
    public class SendService : ISendService
    {
        private readonly ConnectionRabbit _configs;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public SendService(IOptions<ConnectionRabbit> configs)
        {
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                throw new Exception(ex.Message);
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
            _channel.QueueBind(queue: ConstantsRabbit.NameQueue, exchange: ConstantsRabbit.NameExchange, routingKey: ConstantsRabbit.NameQueue);
        }

        #endregion
    }
}