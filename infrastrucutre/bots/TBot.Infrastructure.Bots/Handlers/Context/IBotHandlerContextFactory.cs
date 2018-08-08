using TBot.Infrastructure.Bots.Messages;

namespace TBot.Infrastructure.Bots.Handlers.Context
{
    public interface IBotHandlerContextFactory
    {
        IBotHandlerContext Create<TCommand>(TCommand command)
            where TCommand : class, IBotCommand;
    }
}