using Microsoft.AspNetCore.Mvc;
using RabbitMqClient.Api.Domain;

namespace RabbitMqClient.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMessageController : ControllerBase
    {
        private readonly IRabbitMqClientService _rabbitMqService;

        public SendMessageController(IRabbitMqClientService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        [HttpPost]
        public void Send(ModelDto model)
        {
            _rabbitMqService.PublishMessage(model);
        }
    }
}