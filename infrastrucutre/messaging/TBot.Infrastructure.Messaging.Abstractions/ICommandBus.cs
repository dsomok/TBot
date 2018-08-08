using System;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    public interface ICommandBus
    {
        Task<ISubscription> RegisterHandler<TCommand>(string service, Func<TCommand, Task> handler) where TCommand : class, ICommand;

        Task Send<TCommand>(string service, TCommand command) where TCommand : class, ICommand;

        Task<TResponse> Send<TCommand, TResponse>(string service, TCommand command)
            where TCommand : class, ICommand
            where TResponse : class, IMessage;

        Task<ISubscription> RegisterHandler<TCommand, TResponse>(string service, Func<TCommand, Task<TResponse>> handler)
            where TCommand : class, ICommand 
            where TResponse : class, IMessage;
    }
}