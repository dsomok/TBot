using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    public interface IEventBus
    {
        Task<ISubscription> Subscribe<TEvent>() where TEvent : IEvent;

        Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}