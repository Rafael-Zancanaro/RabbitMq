namespace RabbitMqClient.Api.Domain
{
    public interface IEventService
    {
        Task ProcessEventAsync(string message);
    }
}