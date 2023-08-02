using RabbitMQ.Client;
using RabbitMqClient.Api.Domain;
using System.Text;
using System.Text.Json;

namespace RabbitMqClient.Api.Services
{
    public class RabbitMqClientService : IRabbitMqClientService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqClientService(IConfiguration configuration)
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
        }

        public void PublishMessage(ModelDto model)
        {
            string mensagem = JsonSerializer.Serialize(model);
            var body = Encoding.UTF8.GetBytes(mensagem);

            _channel.BasicPublish(exchange: "NameExchange",
                routingKey: "NameQueue",
                basicProperties: null,
                body: body);
        }
    }
}