using System.Threading.Tasks;
using TBot.Infrastructure.Bots.Contracts.Messages;
using TBot.Infrastructure.Bots.Handlers.Context;

namespace TBot.Infrastructure.Bots.Handlers
{
    public abstract class BotCommandHandler<TCommand> : IBotCommandHandler<TCommand>
        where TCommand : class, IBotCommand
    {
        private readonly IBotHandlerContextFactory _botHandlerContextFactory;


        protected BotCommandHandler(IBotHandlerContextFactory botHandlerContextFactory)
        {
            _botHandlerContextFactory = botHandlerContextFactory;
        }


        public Task Handle(TCommand command)
        {
            var context = this._botHandlerContextFactory.Create(command);
            return this.Handle(command, context);
        }

        public abstract Task Handle(TCommand command, IBotHandlerContext context);
    }
}