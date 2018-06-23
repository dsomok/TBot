using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TBot.Api.Models;

namespace TBot.Api.Controllers
{
    [Route("/")]
    public class WebhookController : Controller
    {
        private readonly ILogger _logger;


        public WebhookController(ILogger logger)
        {
            _logger = logger;
        }

        
        [HttpPost]
        public async Task<IActionResult> GetUpdates([FromBody] UpdateModel update)
        {
            this._logger.Debug("Received update @{Update}", update);
            return Ok();
        }
    }
}
