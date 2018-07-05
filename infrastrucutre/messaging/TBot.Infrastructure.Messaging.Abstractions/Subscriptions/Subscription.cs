using System;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Subscriptions
{
    class Subscription<TMessage> : ISubscription where TMessage : class, IMessage
    {
        private readonly ISubscriptionsRegistry _registry;
        private readonly Func<IMessage, Task> _handler;


        public Subscription(ISubscriptionsRegistry registry, Func<TMessage, Task> handler)
        {
            Id = Guid.NewGuid();
            _registry = registry;
            _handler = message => this.Handle(message, handler);
        }


        public Guid Id { get; }

        public Type Type => typeof(TMessage);

        
        public Task Handle(IMessage message)
        {
            return this._handler.Invoke(message);
        }
        

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