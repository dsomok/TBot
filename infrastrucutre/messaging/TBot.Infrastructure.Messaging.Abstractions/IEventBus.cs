using System;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    public interface IEventBus
    {
        Task<ISubscription> Subscribe<TEvent>(string service, Func<TEvent, Task> handler) where TEvent : class, IEvent;

        Task Publish<TEvent>(TEvent @event) where TEvent : class, IEvent;
    }
}