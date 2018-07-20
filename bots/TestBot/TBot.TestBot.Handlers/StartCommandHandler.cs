using System.Threading.Tasks;
using Serilog;
using TBot.Infrastructure.Bots.Contracts.Commands;
using TBot.Infrastructure.Messaging.Abstractions.Handlers;

namespace TBot.TestBot.Handlers
{
    public class StartCommandHandler : ICommandHandler<StartCommand>
    {
        private readonly ILogger _logger;


        public StartCommandHandler(ILogger logger)
        {
            _logger = logger;
        }


        public Task Handle(StartCommand command)
        {
            this._logger.Information("{CommandType} command has been received", command.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
