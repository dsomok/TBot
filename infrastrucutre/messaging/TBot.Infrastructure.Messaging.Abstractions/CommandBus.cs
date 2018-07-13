using System;
using System.Data;
using System.Threading.Tasks;
using Serilog;
using TBot.Infrastructure.Messaging.Abstractions.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;
using TBot.Infrastructure.Messaging.Abstractions.Topology;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    class CommandBus : ICommandBus
    {
        private readonly IMessageBuilder _messageBuilder;
        private readonly ISubscriptionsRegistry _subscriptionsRegistry;
        private readonly ITopology _topology;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;



        public CommandBus
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


        public async Task<ISubscription> RegisterHandler<TCommand>(string service, Func<TCommand, Task> handler) 
            where TCommand : class, ICommand
        {
            var endpoint = this._topology.ResolveSubscriptionEndpoint<TCommand>(service);
            await endpoint.Subscribe(OnCommand);

            var subscription = this._subscriptionsRegistry.CreateSubscription(endpoint, handler);
            return subscription;
        }

        public Task Send<TCommand>(string service, TCommand command) 
            where TCommand : class, ICommand
        {
            var topic = $"Command.{service}.{typeof(TCommand).Name}";
            var message = this._messageBuilder.Build(topic, command);

            var endpoint = this._topology.ResolvePublishingEndpoint<TCommand>(service, message);
            return endpoint.Publish(message);
        }



        private async Task<bool> OnCommand(Message message)
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

            if (subscriptions.Count > 1)
            {
                this._logger.Warning("Command of type {CommandType} was received but multiple handler for this type were registered.", commandType);
                throw new InvalidConstraintException("Commands cannot have multiple handlers");
            }

            var subscription = subscriptions[0];

            await subscription.Handle(command);
            return true;
        }
    }
}