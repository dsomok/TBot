using RabbitMQ.Client;
using TBot.Infrastructure.Hosting.Abstractions;
using TBot.Infrastructure.Messaging.Abstractions;
using TBot.Infrastructure.Messaging.Abstractions.Endpoints;
using TBot.Infrastructure.Messaging.Abstractions.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Topology;
using TBot.Infrastructure.Messaging.RabbitMQ.Endpoints;

namespace TBot.Infrastructure.Messaging.RabbitMQ.Topology
{
    class SimpleRabbitMQTopology : ITopology
    {
        private const string EVENTS_EXCHANGE = "Events";
        private const string COMMANDS_EXCHANGE = "Commands";
        
        private readonly RabbitMQExchangeEndpoint _eventsExchange;
        private readonly RabbitMQExchangeEndpoint _commandsExchange;

        private readonly IEndpointRegistry _endpointRegistry;
        private readonly ConnectionFactory _connectionFactory;


        public SimpleRabbitMQTopology(IEndpointRegistry endpointRegistry, ConnectionFactory connectionFactory)
        {
            _endpointRegistry = endpointRegistry;
            _connectionFactory = connectionFactory;

            this._eventsExchange = new RabbitMQExchangeEndpoint(EVENTS_EXCHANGE, "direct", this._connectionFactory);
            this._endpointRegistry.Add(this._eventsExchange);
            this._commandsExchange = new RabbitMQExchangeEndpoint(COMMANDS_EXCHANGE, "direct", this._connectionFactory);
            this._endpointRegistry.Add(this._commandsExchange);
        }


        public string GetEventTopic<TEvent>() where TEvent : IEvent 
            => $"Event.{typeof(TEvent).Name}";

        public string GetCommandTopic<TCommand>(string service) where TCommand : ICommand 
            => $"Command.{service}.{typeof(TCommand).Name}";


        public IEndpoint ResolveEventSubscriptionEndpoint<TEvent>(string service) where TEvent : IEvent
        {
            var queueName = $"{service}.{typeof(TEvent).FullName}";
            if (!this._endpointRegistry.TryGet(queueName, out RabbitMQQueueEndpoint queueEndpoint))
            {
                queueEndpoint = new RabbitMQQueueEndpoint(queueName, this._connectionFactory);
                queueEndpoint.Bind(
                    exchangeName: EVENTS_EXCHANGE,
                    routingKey: this.GetEventTopic<TEvent>()
                );

                this._endpointRegistry[queueName] = queueEndpoint;
            }

            return queueEndpoint;
        }

        public IEndpoint ResolveEventPublishingEndpoint<TEvent>(Message message) where TEvent : IEvent
        {
            return this._eventsExchange;
        }

        
        public IEndpoint ResolveCommandSubscriptionEndpoint<TCommand>(string service) where TCommand : ICommand
        {
            var queueName = $"{service}.{typeof(TCommand).FullName}";
            if (!this._endpointRegistry.TryGet(queueName, out RabbitMQQueueEndpoint queueEndpoint))
            {
                queueEndpoint = new RabbitMQQueueEndpoint(queueName, this._connectionFactory);
                queueEndpoint.Bind(
                    exchangeName: COMMANDS_EXCHANGE,
                    routingKey: this.GetCommandTopic<TCommand>(service)
                );

                this._endpointRegistry[queueName] = queueEndpoint;
            }

            return queueEndpoint;
        }

        public IEndpoint ResolveCommandPublishingEndpoint<TCommand>(string service, Message message) where TCommand : ICommand
        {
            return this._commandsExchange;
        }

        public IEndpoint ResolveCommandReplyToEndpoint<TResponse>(HostContext hostContext) where TResponse : IMessage
        {
            var queueName = $"{hostContext.ServiceName}.Responses.{typeof(TResponse).FullName}";
            if (!this._endpointRegistry.TryGet(queueName, out RabbitMQQueueEndpoint queueEndpoint))
            {
                queueEndpoint = new RabbitMQQueueEndpoint(queueName, this._connectionFactory);
                this._endpointRegistry[queueName] = queueEndpoint;
            }

            return queueEndpoint;
        }
        

        public void Dispose()
        {
            this._endpointRegistry.Dispose();
        }
    }
}
