using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TBot.Api.Models;
using TBot.Infrastructure.Bots.Contracts.Commands;
using TBot.Infrastructure.Messaging.Abstractions;

namespace TBot.Api.Controllers
{
    [Route("/")]
    public class WebhookController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly ILogger _logger;


        public WebhookController(
            ICommandBus commandBus, 
            ILogger logger
        )
        {
            _logger = logger;
            _commandBus = commandBus;
        }

        
        [HttpPost]
        [Route("{name}")]
        public async Task<IActionResult> GetUpdates(string name, [FromBody] UpdateModel update)
        {
            var command = new StartCommand(
                chatId: update.Message.Chat.Id,
                messageId: update.Message.MessageId
            );

            await this._commandBus.Send(name, command);

            return Ok();
        }

    }
}
