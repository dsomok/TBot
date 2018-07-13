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
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;


        public EventBus
        (
            IMessageBuilder messageBuilder, 
            ISubscriptionsRegistry subscriptionsRegistry, 
            ITopology topology, 
            ISerializer serializer, 
            ILogger logger
        )
        {
            _messageBuilder = messageBuilder;
            _subscriptionsRegistry = subscriptionsRegistry;
            _topology = topology;
            _serializer = serializer;
            _logger = logger;
        }


        public async Task<ISubscription> Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class, IEvent
        {
            var endpoint = this._topology.ResolveSubscriptionEndpoint<TEvent>();
            await endpoint.Subscribe(OnEvent);

            var subscription = this._subscriptionsRegistry.CreateSubscription(endpoint, handler);
            return subscription;
        }

        public Task Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
        {
            var topic = $"Event.{typeof(TEvent).Name}";
            var message = this._messageBuilder.Build(topic, @event);

            var endpoint = this._topology.ResolvePublishingEndpoint<TEvent>(message);
            return endpoint.Publish(message);
        }



        private async Task<bool> OnEvent(Message message)
        {
            var eventType = message.BodyType;
            var @event = this._serializer.Deserialize(message.Body) as IMessage;
            if (@event == null)
            {
                this._logger.Warning("Failed to deserialize event of type {EventType}", eventType);
                throw new InvalidCastException($"Failed to deserialize event of type {eventType}");
            }

            var subscriptions = this._subscriptionsRegistry.ResolveSubscriptions(eventType);

            if (subscriptions == null)
            {
                this._logger.Warning("Command of type {CommandType} was received but no handler for this type were registered.", eventType);
                return false;
            }

            await Task.WhenAll(
                subscriptions.Select(subscription => subscription.Handle(@event))
            );

            return true;
        }
    }
}