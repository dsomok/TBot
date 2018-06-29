using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Subscriptions
{
    public interface ISubscriptionsRegistry
    {
        ISubscription CreateSubscription<TMessage>(Func<TMessage, Task> handler) where TMessage : class, IMessage;

        ISubscription ResolveSubscription<TMessage>();

        void DeleteSubscription(ISubscription subscription);
    }

    class SubscriptionsRegistry : ISubscriptionsRegistry
    {
        private readonly IDictionary<string, ISubscription> _subscriptions = new Dictionary<string, ISubscription>();


        public ISubscription CreateSubscription<TMessage>(Func<TMessage, Task> handler) where TMessage : class, IMessage
        {
            var subscription = new Subscription<TMessage>(this, handler);
            this._subscriptions.Add(typeof(TMessage).Name, subscription);

            return subscription;
        }

        public ISubscription ResolveSubscription<TMessage>()
        {
            var key = typeof(TMessage).Name;
            return this._subscriptions[key];
        }

        public void DeleteSubscription(ISubscription subscription)
        {
            var key = subscription.Type.Name;
            this._subscriptions.Remove(key);
        }
    }
}