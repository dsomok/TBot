using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TBot.Api.Models;
using TBot.Infrastructure.Bots.Contracts.Commands;
using TBot.Infrastructure.Messaging.Abstractions;
using TBot.TestBot.Contracts.Responses;

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
            this._logger.Information("Received update @{Update}", update);

            var command = new StartCommand();
            var response = await this._commandBus.Send<StartCommand, TestBotResponse>(name, command);

            return Ok(response.Message);
        }

    }
}
