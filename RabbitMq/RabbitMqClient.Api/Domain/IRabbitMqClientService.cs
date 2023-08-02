namespace RabbitMqClient.Api.Domain
{
    public interface IRabbitMqClientService
    {
        void PublishMessage(ModelDto model);
    }
}