using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqClient.Api.Domain;
using System.Text;

namespace RabbitMqClient.Api
{
    public class RabbitMqSubscribeService : BackgroundService
    {
        private readonly IEventService _eventService;
        private readonly ILogger<RabbitMqSubscribeService> _logger;
        private readonly ConnectionRabbit _config;
        private IConnection _connection;
        private IModel _channel;


        public RabbitMqSubscribeService(IOptions<ConnectionRabbit> options, ILogger<RabbitMqSubscribeService> logger, IEventService eventService)
        {
            _logger = logger;
            _config = options.Value;
            _eventService = eventService;
            IniciarRabbitMq();
        }

        public void IniciarRabbitMq()
        {
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
            _channel.QueueDeclare(ConstantsRabbit.NameQueue, true, false, false, null);
            _channel.QueueBind(ConstantsRabbit.NameQueue, ConstantsRabbit.NameExchange, ConstantsRabbit.NameQueue);
            _channel.BasicQos(0, 5, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var message = string.Empty;
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, args) =>
            {
                try
                {
                    var body = args.Body.ToArray();
                    message = Encoding.UTF8.GetString(body);

                    await _eventService.ProcessEventAsync(message);

                    _channel.BasicAck(args.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ConstantsRabbit.ErrorRabbit, string.Format(RabbitResources.ErrorConsuming, ConstantsRabbit.NameQueue, message));
                    _channel.BasicNack(args.DeliveryTag, false, !args.Redelivered);
                }
            };

            _channel.BasicConsume(queue: ConstantsRabbit.NameQueue, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _connection.Close();
            _channel.Close();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}