namespace RabbitMqClient.Api.Domain
{
    public class ConnectionRabbit
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Environment { get; set; }
        public int Port { get; set; }
    }
}