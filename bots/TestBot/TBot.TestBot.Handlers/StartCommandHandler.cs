using System.Threading.Tasks;
using Serilog;
using TBot.Infrastructure.Bots.Contracts.Commands;
using TBot.Infrastructure.Messaging.Abstractions.Handlers;
using TBot.TestBot.Contracts.Responses;

namespace TBot.TestBot.Handlers
{
    public class StartCommandHandler : ICommandHandler<StartCommand, TestBotResponse>
    {
        private readonly ILogger _logger;


        public StartCommandHandler(ILogger logger)
        {
            _logger = logger;
        }


        public Task<TestBotResponse> Handle(StartCommand command)
        {
            this._logger.Information("{CommandType} command has been received", command.GetType().Name);

            var response = new TestBotResponse("Hi");

            return Task.FromResult(response);
        }
    }
}
