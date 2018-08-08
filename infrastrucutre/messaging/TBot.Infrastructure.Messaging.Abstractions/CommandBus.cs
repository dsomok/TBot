using System;
using System.Data;
using System.Threading.Tasks;
using Serilog;
using TBot.Infrastructure.Hosting.Abstractions;
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
        private readonly ILogger _logger;
        private readonly HostContext _hostContext;



        public CommandBus
        (
            IMessageBuilder messageBuilder,
            ISubscriptionsRegistry subscriptionsRegistry, 
            ITopology topology,
            ILogger logger,
            HostContext hostContext
        )
        {
            _messageBuilder = messageBuilder;
            _subscriptionsRegistry = subscriptionsRegistry;
            _topology = topology;
            _logger = logger;
            _hostContext = hostContext;
        }


        public async Task<ISubscription> RegisterHandler<TCommand>(string service, Func<TCommand, Task> handler) 
            where TCommand : class, ICommand
        {
            var endpoint = this._topology.ResolveCommandSubscriptionEndpoint<TCommand>(service);
            if (!endpoint.IsSubscribed)
            {
                await endpoint.Subscribe(OnCommand);
            }

            var subscription = this._subscriptionsRegistry.CreateSubscription<TCommand>(endpoint, async command =>
            {
                try
                {
                    await handler(command);
                    return true;
                }
                catch (Exception ex)
                {
                    this._logger.Warning(ex, "Failed to handle {CommandType} command.", typeof(TCommand).Name);
                    return false;
                }
            });
            return subscription;
        }

        public async Task<ISubscription> RegisterHandler<TCommand, TResponse>(string service, Func<TCommand, Task<TResponse>> handler)
            where TCommand : class, ICommand 
            where TResponse : class, IMessage
        {
            var endpoint = this._topology.ResolveCommandSubscriptionEndpoint<TCommand>(service);
            if (!endpoint.IsSubscribed)
            {
                await endpoint.Subscribe(OnCommand);
            }

            var subscription = this._subscriptionsRegistry.CreateSubscription<TCommand>(endpoint, async (command, message) =>
            {
                try
                {
                    var response = await handler(command);

                    var responseTopic = this._topology.GetResponseTopic<TResponse>();
                    var responseMessage = this._messageBuilder.Build(responseTopic, message.CorrelationId, response);

                    var replyToEndpoint = this._topology.ResolveCommandReplyToEndpoint(message.ReplyTo);
                    await replyToEndpoint.Publish(responseMessage);

                    return true;
                }
                catch (Exception ex)
                {
                    this._logger.Warning(ex, "Failed to handle {CommandType} command.", typeof(TCommand).Name);
                    return false;
                }
            });

            return subscription;
        }


        public Task Send<TCommand>(string service, TCommand command) 
            where TCommand : class, ICommand
        {
            var topic = this._topology.GetCommandTopic<TCommand>(service);
            var message = this._messageBuilder.Build(topic, command);

            var endpoint = this._topology.ResolveCommandPublishingEndpoint<TCommand>(service, message);
            return endpoint.Publish(message);
        }

        public Task<TResponse> Send<TCommand, TResponse>(string service, TCommand command) 
            where TCommand : class, ICommand 
            where TResponse : class, IMessage
        {
            var topic = this._topology.GetCommandTopic<TCommand>(service);
            var replyToEndpoint = this._topology.ResolveCommandReplyToEndpoint<TCommand>(this._hostContext);
            var message = this._messageBuilder.Build(topic, command, replyToEndpoint);

            var responseAwaiter = new ResponseAwaiter<TResponse>(
                correlationId: message.CorrelationId,
                subscriptionsRegistry: this._subscriptionsRegistry,
                responseEndpoint: replyToEndpoint
            );

            var endpoint = this._topology.ResolveCommandPublishingEndpoint<TCommand>(service, message);
            endpoint.Publish(message);

            return responseAwaiter.WaitAsync();
        }


        private async Task<bool> OnCommand(Message message)
        {
            var commandType = message.BodyType;

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

            return await subscription.Handle(message);
        }
    }
}