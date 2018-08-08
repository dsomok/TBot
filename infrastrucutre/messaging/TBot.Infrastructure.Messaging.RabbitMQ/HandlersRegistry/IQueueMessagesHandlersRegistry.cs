using System;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions;

namespace TBot.Infrastructure.Messaging.RabbitMQ.HandlersRegistry
{
    interface IQueueMessagesHandlersRegistry
    {
        bool Any();

        Task ForEach(Func<Func<Message, Task<bool>>, Task> action);

        Task<Guid> Add(Func<Message, Task<bool>> handler);

        Task Remove(Guid subscriptionId);
    }
}