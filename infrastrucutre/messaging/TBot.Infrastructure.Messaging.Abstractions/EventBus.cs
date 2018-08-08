using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using TBot.Infrastructure.Messaging.Abstractions.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;
using TBot.Infrastructure.Messaging.Abstractions.Topology;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    class EventBus : IEventBus
    {
        private readonly IMessageBuilder _messageBuilder;
        private readonly ISubscriptionsRegistry _subscriptionsRegistry;
        private readonly ITopology _topology;
        private readonly ILogger _logger;


        public EventBus
        (
            IMessageBuilder messageBuilder, 
            ISubscriptionsRegistry subscriptionsRegistry, 
            ITopology topology,
            ILogger logger
        )
        {
            _messageBuilder = messageBuilder;
            _subscriptionsRegistry = subscriptionsRegistry;
            _topology = topology;
            _logger = logger;
        }


        public async Task<ISubscription> Subscribe<TEvent>(string service, Func<TEvent, Task> handler) where TEvent : class, IEvent
        {
            var endpoint = this._topology.ResolveEventSubscriptionEndpoint<TEvent>(service);
            if (!endpoint.IsSubscribed)
            {
                await endpoint.Subscribe(OnEvent);
            }

            var subscription = this._subscriptionsRegistry.CreateSubscription<TEvent>(endpoint, async @event =>
            {
                try
                {
                    await handler(@event);
                    return true;
                }
                catch (Exception ex)
                {
                    this._logger.Warning(ex, "Failed to handle {EventType} event.", typeof(TEvent).Name);
                    return false;
                }
            });
            return subscription;
        }

        public Task Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
        {
            var topic = this._topology.GetEventTopic<TEvent>();
            var message = this._messageBuilder.Build(topic, @event);

            var endpoint = this._topology.ResolveEventPublishingEndpoint<TEvent>(message);
            return endpoint.Publish(message);
        }



        private async Task<bool> OnEvent(Message message)
        {
            var eventType = message.BodyType;

            var subscriptions = this._subscriptionsRegistry.ResolveSubscriptions(eventType);

            if (subscriptions == null)
            {
                this._logger.Warning("Command of type {CommandType} was received but no handler for this type were registered.", eventType);
                return false;
            }

            await Task.WhenAll(
                subscriptions.Select(subscription => subscription.Handle(message))
            );

            return true;
        }
    }
}