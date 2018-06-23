using System.Threading.Tasks;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    public interface IEventBus
    {
        Task Subscribe<TEvent>() where TEvent : IEvent;

        Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}