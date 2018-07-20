using System;
using System.Threading.Tasks;

namespace TBot.Infrastructure.Messaging.Abstractions.Subscriptions
{
    public interface ISubscription : IDisposable
    {
        Guid Id { get; }
        Type Type { get; }

        Task<bool> Handle(Message message);
    }
}