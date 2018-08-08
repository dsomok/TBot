using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions;

namespace TBot.Infrastructure.Messaging.RabbitMQ.HandlersRegistry
{
    class QueueMessagesHandlersRegistry : IQueueMessagesHandlersRegistry
    {
        private readonly SemaphoreSlim _handlersSemaphore = new SemaphoreSlim(1, 1);
        private readonly IDictionary<Guid, Func<Message, Task<bool>>> _handlers = new Dictionary<Guid, Func<Message, Task<bool>>>();

        public bool Any()
        {
            return this._handlers.Any();
        }

        public Task ForEach(Func<Func<Message, Task<bool>>, Task> action)
        {
            return this.SynchronizeHandlersAccess(async () =>
            {
                foreach (var handler in this._handlers.Values)
                {
                    await action(handler);
                }
            });
        }

        public async Task<Guid> Add(Func<Message, Task<bool>> handler)
        {
            var subscriptionId = Guid.NewGuid();

            await this.SynchronizeHandlersAccess(() =>
            {
                this._handlers.Add(subscriptionId, handler);
                return Task.CompletedTask;
            });

            return subscriptionId;
        }

        public Task Remove(Guid subscriptionId)
        {
            return this.SynchronizeHandlersAccess(() =>
            {
                this._handlers.Remove(subscriptionId);
                return Task.CompletedTask;
            });
        }

        private async Task SynchronizeHandlersAccess(Func<Task> action)
        {
            await _handlersSemaphore.WaitAsync();
            try
            {
                await action();
            }
            finally
            {
                _handlersSemaphore.Release();
            }
        }
    }
}