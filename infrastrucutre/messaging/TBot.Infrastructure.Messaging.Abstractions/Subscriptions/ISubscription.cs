using System;
using System.Reflection;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Subscriptions
{
    public interface ISubscription : IDisposable
    {
        Guid Id { get; }

        Type Type { get; }

        Func<IMessage, Task> Handler { get; } 
    }

    class Subscription<TMessage> : ISubscription where TMessage : class, IMessage
    {
        private readonly ISubscriptionsRegistry _registry;


        public Subscription(ISubscriptionsRegistry registry, Func<TMessage, Task> handler)
        {
            Id = Guid.NewGuid();
            _registry = registry;
            Handler = message => this.Handle(message, handler);
        }


        public Guid Id { get; }

        public Type Type => typeof(TMessage);

        public Func<IMessage, Task> Handler { get; }


        private Task Handle(IMessage message, Func<TMessage, Task> handler)
        {
            if (!(message is TMessage tMessage))
            {
                throw new InvalidCastException($"This subscription is not intended to handle messages of type {message.GetType().Name}");
            }

            return handler(tMessage);
        }
        

        public void Dispose()
        {
            this._registry.DeleteSubscription(this);
        }
    }
}