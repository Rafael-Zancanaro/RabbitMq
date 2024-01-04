namespace RabbitMqClient.Receive.Domain
{
    public interface IReceiveService
    {
        Task ProcessEventAsync(string message);
    }
}