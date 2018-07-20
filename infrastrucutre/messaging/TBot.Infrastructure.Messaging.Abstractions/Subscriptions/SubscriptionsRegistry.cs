using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using TBot.Infrastructure.Messaging.Abstractions.Endpoints;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Subscriptions
{
    class SubscriptionsRegistry : ISubscriptionsRegistry
    {
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;

        private readonly IDictionary<string, IList<ISubscription>> _subscriptions = new Dictionary<string, IList<ISubscription>>();


        public SubscriptionsRegistry(ISerializer serializer, ILogger logger)
        {
            _serializer = serializer;
            _logger = logger;
        }


        public ISubscription CreateSubscription<TMessage>(IEndpoint endpoint, Func<TMessage, Task<bool>> handler) where TMessage : class, IMessage
        {
            return this.CreateSubscription(
                endpoint,
                (TMessage response, Message message) => handler(response)
            );
        }

        public ISubscription CreateSubscription<TMessage>(IEndpoint endpoint, Func<TMessage, Message, Task<bool>> handler) where TMessage : class, IMessage
        {
            var subscription = new Subscription<TMessage>(this, this._serializer, this._logger, endpoint, handler);

            var key = this.GetKey<TMessage>();

            if (this._subscriptions.TryGetValue(key, out IList<ISubscription> subscriptions))
            {
                subscriptions.Add(subscription);
            }
            else
            {
                this._subscriptions.Add(key, new List<ISubscription> { subscription });
            }

            return subscription;
        }

        public IList<ISubscription> ResolveSubscriptions<TMessage>()
        {
            return this.ResolveSubscriptions(this.GetKey<TMessage>());
        }

        public IList<ISubscription> ResolveSubscriptions(string messageType)
        {
            var key = messageType;

            if (this._subscriptions.TryGetValue(key, out IList<ISubscription> subscriptions))
            {
                return subscriptions;
            }

            return null;
        }

        public void DeleteSubscription(ISubscription subscription)
        {
            var key = this.GetKey(subscription.Type);

            if (this._subscriptions.TryGetValue(key, out IList<ISubscription> subscriptions))
            {
                subscriptions.Remove(subscription);
            }
        }



        private string GetKey<TMessage>()
        {
            return this.GetKey(typeof(TMessage));
        }

        private string GetKey(Type messageType)
        {
            return messageType.FullName;
        }
    }
}