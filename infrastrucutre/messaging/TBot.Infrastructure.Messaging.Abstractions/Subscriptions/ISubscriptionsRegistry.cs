using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Endpoints;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Subscriptions
{
    public interface ISubscriptionsRegistry
    {
        ISubscription CreateSubscription<TMessage>(IEndpoint endpoint, Func<TMessage, Task<bool>> handler) 
            where TMessage : class, IMessage;
        ISubscription CreateSubscription<TMessage>(IEndpoint endpoint, Func<TMessage, Message, Task<bool>> handler) 
            where TMessage : class, IMessage;

        IList<ISubscription> ResolveSubscriptions<TMessage>();
        IList<ISubscription> ResolveSubscriptions(string messageType);

        void DeleteSubscription(ISubscription subscription);
    }
}