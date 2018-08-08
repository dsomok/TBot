using System;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Endpoints;
using TBot.Infrastructure.Messaging.Abstractions.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    internal class ResponseAwaiter<TResponse>
        where TResponse : class, IMessage
    {
        private readonly Guid _correlationId;
        private readonly ISubscriptionsRegistry _subscriptionsRegistry;
        private readonly IEndpoint _responseEndpoint;
        private readonly TaskCompletionSource<TResponse> _completionSource = new TaskCompletionSource<TResponse>();


        public ResponseAwaiter(Guid correlationId, ISubscriptionsRegistry subscriptionsRegistry, IEndpoint responseEndpoint)
        {
            _correlationId = correlationId;
            _subscriptionsRegistry = subscriptionsRegistry;
            _responseEndpoint = responseEndpoint;
        }


        public async Task<TResponse> WaitAsync()
        {
            var responseSubscription = this._subscriptionsRegistry.CreateSubscription<TResponse>(
                this._responseEndpoint,
                (response, responseMessage) =>
                {
                        this._completionSource.SetResult(response);
                        return Task.FromResult(true);
                }
            );

            var subscriptionId = await this._responseEndpoint.Subscribe(responseMessage =>
            {
                if (responseMessage.CorrelationId == this._correlationId)
                {
                    return responseSubscription.Handle(responseMessage);
                }

                return Task.FromResult(false);
            });

            try
            {
                var result = await this._completionSource.Task;
                return result;
            }
            finally
            {
                await this._responseEndpoint.Unsubscribe(subscriptionId);
                responseSubscription.Dispose();
            }
        }
    }
}
