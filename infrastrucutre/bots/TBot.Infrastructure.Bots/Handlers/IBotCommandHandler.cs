using System.Threading.Tasks;
using TBot.Infrastructure.Bots.Contracts.Messages;
using TBot.Infrastructure.Bots.Handlers.Context;
using TBot.Infrastructure.Messaging.Abstractions.Handlers;

namespace TBot.Infrastructure.Bots.Handlers
{
    public interface IBotCommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, IBotCommand
    {
        Task Handle(TCommand command, IBotHandlerContext context);
    }
}