using System;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Subscriptions
{
    public interface ISubscription : IDisposable
    {
        Guid Id { get; }
        Type Type { get; }

        Task Handle(IMessage message);
    }
}