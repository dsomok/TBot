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
            await endpoint.Subscribe(OnCommand);

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

        public Task Send<TCommand>(string service, TCommand command) 
            where TCommand : class, ICommand
        {
            var topic = this._topology.GetCommandTopic<TCommand>(service);
            var message = this._messageBuilder.Build(topic, command);

            var endpoint = this._topology.ResolveCommandPublishingEndpoint<TCommand>(service, message);
            return endpoint.Publish(message);
        }

        public async Task<TResponse> Send<TCommand, TResponse>(string service, TCommand command) 
            where TCommand : class, ICommand 
            where TResponse : class, IMessage
        {
            var topic = this._topology.GetCommandTopic<TCommand>(service);
            var replyToEndpoint = this._topology.ResolveCommandReplyToEndpoint<TCommand>(this._hostContext);
            var message = this._messageBuilder.Build(topic, command, replyToEndpoint);

            var responseAwaiter = new TaskCompletionSource<TResponse>();

            var responseSubscription = this._subscriptionsRegistry.CreateSubscription<TResponse>(
                replyToEndpoint,
                (response, responseMessage) =>
                {
                    if (responseMessage.ReplyToMessage == message.CorrelationId)
                    {
                        responseAwaiter.SetResult(response);
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                }
            );

            using (responseSubscription)
            {
                var endpoint = this._topology.ResolveCommandPublishingEndpoint<TCommand>(service, message);
                await endpoint.Publish(message);

                return await responseAwaiter.Task;
            }
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