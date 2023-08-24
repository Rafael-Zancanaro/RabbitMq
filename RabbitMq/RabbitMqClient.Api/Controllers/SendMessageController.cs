using MediatR;
using Microsoft.AspNetCore.Mvc;
using RabbitMqClient.Api.Events.Notifications;

namespace RabbitMqClient.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMessageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SendMessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async void Send(EventNotification model)
        {
            await _mediator.Publish(model);
        }
    }
}