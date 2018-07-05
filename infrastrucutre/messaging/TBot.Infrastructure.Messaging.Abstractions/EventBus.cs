using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using TBot.Infrastructure.Messaging.Abstractions.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    class EventBus : IEventBus
    {
        private readonly IMessageBuilder _messageBuilder;
        private readonly ISubscriptionsRegistry _subscriptionsRegistry;
        private readonly IPublisher _publisher;
        private readonly ISubscriber _subscriber;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;


        public EventBus(
            IMessageBuilder messageBuilder, 
            ISubscriptionsRegistry subscriptionsRegistry, 
            IPublisher publisher, 
            ISubscriber subscriber, 
            ISerializer serializer, 
            ILogger logger
        )
        {
            _messageBuilder = messageBuilder;
            _subscriptionsRegistry = subscriptionsRegistry;
            _publisher = publisher;
            _subscriber = subscriber;
            _serializer = serializer;
            _logger = logger;
        }



        public Task SubscribeForAllMessages()
        {
            return this._subscriber.Subscribe(this.OnEvent);
        }

        public Task<ISubscription> Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class, IEvent
        {
            var subscription = this._subscriptionsRegistry.CreateSubscription(handler);
            return Task.FromResult(subscription);
        }

        public Task Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
        {
            var topic = $"Event.{typeof(TEvent).Name}";
            var message = this._messageBuilder.Build(topic, @event);

            return this._publisher.Publish(message);
        }



        private async Task<bool> OnEvent(Message message)
        {
            var commandType = message.BodyType;
            var command = this._serializer.Deserialize(message.Body) as IMessage;
            if (command == null)
            {
                this._logger.Warning("Failed to deserialize command of type {CommandType}", commandType);
                throw new InvalidCastException($"Failed to deserialize command of type {commandType}");
            }

            var subscriptions = this._subscriptionsRegistry.ResolveSubscriptions(commandType);

            if (subscriptions == null)
            {
                this._logger.Warning("Command of type {CommandType} was received but no handler for this type were registered.", commandType);
                return false;
            }

            await Task.WhenAll(
                subscriptions.Select(subscription => subscription.Handle(command))
            );

            return true;
        }
    }
}