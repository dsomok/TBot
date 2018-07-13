using System;
using System.Collections.Generic;
using TBot.Infrastructure.Messaging.Abstractions;
using TBot.Infrastructure.Messaging.Abstractions.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Topology;

namespace TBot.Infrastructure.Messaging.RabbitMQ
{
    class SimpleRabbitMQTopology : ITopology
    {
        private const string EVENTS_EXCHANGE = "Events";
        private const string COMMANDS_EXCHANGE = "Commands";

        private readonly IDictionary<string, RabbitMQQueueEndpoint> _queues = new Dictionary<string, RabbitMQQueueEndpoint>();
        private readonly IDictionary<string, RabbitMQExchangeEndpoint> _exchanges = new Dictionary<string, RabbitMQExchangeEndpoint>();


        public SimpleRabbitMQTopology()
        {
            var commandsExchange = new RabbitMQExchangeEndpoint(COMMANDS_EXCHANGE, "direct");
            var eventsExchange = new RabbitMQExchangeEndpoint(EVENTS_EXCHANGE, "");
        }


        public IEndpoint ResolveEventSubscriptionEndpoint<TMessage>(string service) where TMessage : IEvent
        {
            var queueName = $"{service}.{typeof(TMessage).FullName}";
            if (!this._queues.TryGetValue(queueName, out RabbitMQQueueEndpoint queueEndpoint))
            {
                queueEndpoint = new RabbitMQQueueEndpoint(queueName);
                queueEndpoint.Bind(
                    exchangeName: EVENTS_EXCHANGE,
                    routingKey: queueName
                );

                this._queues[queueName] = queueEndpoint;
            }

            return queueEndpoint;
        }

        public IEndpoint ResolveCommandSubscriptionEndpoint<TMessage>(string service) where TMessage : ICommand
        {
            var queueName = $"{service}.{typeof(TMessage).FullName}";
            if (!this._queues.TryGetValue(queueName, out RabbitMQQueueEndpoint queueEndpoint))
            {
                queueEndpoint = new RabbitMQQueueEndpoint(queueName);
                queueEndpoint.Bind(
                    exchangeName: COMMANDS_EXCHANGE,
                    routingKey: queueName
                );

                this._queues[queueName] = queueEndpoint;
            }

            return queueEndpoint;
        }

        public IEndpoint ResolveEventPublishingEndpoint<TMessage>(Message message) where TMessage : IEvent
        {
            throw new NotImplementedException();
        }

        public IEndpoint ResolveCommandPublishingEndpoint<TMessage>(string service, Message message) where TMessage : ICommand
        {
            throw new NotImplementedException();
        }
    }
}
