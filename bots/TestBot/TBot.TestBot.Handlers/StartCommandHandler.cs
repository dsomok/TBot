using System.Threading.Tasks;
using Serilog;
using TBot.Infrastructure.Bots.Contracts.Commands;
using TBot.Infrastructure.Bots.Handlers;
using TBot.Infrastructure.Bots.Handlers.Context;

namespace TBot.TestBot.Handlers
{
    public class StartCommandHandler : BotCommandHandler<StartCommand>
    {
        private readonly ILogger _logger;


        public StartCommandHandler(IBotHandlerContextFactory botHandlerContextFactory, ILogger logger) 
            : base(botHandlerContextFactory)
        {
            _logger = logger;
        }


        public override Task Handle(StartCommand command, IBotHandlerContext context)
        {
            this._logger.Information("{CommandType} command has been received", command.GetType().Name);

            var response = "Hi";

            return context.SendMessage(response);
        }
    }
}
