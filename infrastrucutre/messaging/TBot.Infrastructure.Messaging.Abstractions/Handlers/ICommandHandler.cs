using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Handlers
{
    public interface ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        Task Handle(TCommand command);
    }

    public interface ICommandHandler<TCommand, TResponse>
        where TCommand : class, ICommand
        where TResponse : class, IMessage
    {
        Task<TResponse> Handle(TCommand command);
    }
}