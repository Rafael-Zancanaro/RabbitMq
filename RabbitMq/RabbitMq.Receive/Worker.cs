using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqClient.Receive.Domain;
using RabbitMqClient.Send.Domain;
using System.Text;

namespace RabbitMqClient.Receive
{
    public class Worker : BackgroundService
    {
        private readonly ConnectionRabbit _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider, IOptions<ConnectionRabbit> options)
        {
            _serviceProvider = serviceProvider;
            _config = options.Value;

            var factory = new ConnectionFactory()
            {
                HostName = _config.Host,
                UserName = _config.UserName,
                Password = _config.Password,
                VirtualHost = _config.Environment,
                Port = _config.Port
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            ConfigureQueuesAndExchanges();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, args) =>
            {
                try
                {
                    var body = args.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    using var scope = _serviceProvider.CreateScope();
                    await scope.ServiceProvider.GetRequiredService<IReceiveService>().ProcessEventAsync(message);

                    _channel.BasicAck(args.DeliveryTag, false);
                }
                catch
                {
                    _channel.BasicNack(args.DeliveryTag, false, !args.Redelivered);
                }
            };

            _channel.BasicConsume(queue: ConstantsRabbit.NameQueue, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        #region Metodos Privados

        private void ConfigureQueuesAndExchanges()
        {
            _channel.ExchangeDeclare(ConstantsRabbit.NameExchangeDeadLetter, ExchangeType.Fanout);
            _channel.QueueDeclare(ConstantsRabbit.NameQueueDeadLetter, true, false, false, null);
            _channel.QueueBind(ConstantsRabbit.NameQueueDeadLetter, ConstantsRabbit.NameExchangeDeadLetter, string.Empty);

            var arguments = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", ConstantsRabbit.NameExchangeDeadLetter},
            };

            _channel.QueueDeclare(ConstantsRabbit.NameQueue, true, false, false, arguments);
            _channel.BasicQos(0, 100, false);
        }

        #endregion
    }
}