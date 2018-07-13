using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Subscriptions
{
    public interface ISubscriptionsRegistry
    {
        ISubscription CreateSubscription<TMessage>(IEndpoint endpoint, Func<TMessage, Task> handler) where TMessage : class, IMessage;

        IList<ISubscription> ResolveSubscriptions<TMessage>();
        IList<ISubscription> ResolveSubscriptions(string messageType);

        void DeleteSubscription(ISubscription subscription);
    }
}