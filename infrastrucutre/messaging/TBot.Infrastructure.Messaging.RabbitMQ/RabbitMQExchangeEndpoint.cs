using System;
using System.IO;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TBot.Infrastructure.Messaging.Abstractions;

namespace TBot.Infrastructure.Messaging.RabbitMQ
{
    class RabbitMQExchangeEndpoint : IEndpoint
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;


        public RabbitMQExchangeEndpoint(string name, string type)
        {
            Name = name;
            Type = type;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            this._connection = factory.CreateConnection();
            this._channel = this._connection.CreateModel();
            this._consumer = new EventingBasicConsumer(this._channel);

            this.Create();
        }


        public string Name { get; }

        public string Type { get; }


        public Task Publish(Message message)
        {
            var props = this._channel.CreateBasicProperties();
            props.ReplyTo = message.ReplyTo;
            props.CorrelationId = message.CorrelationId.ToString();
            props.Type = message.BodyType;

            this._channel.BasicPublish(
                exchange: this.Name,
                routingKey: message.Topic,
                basicProperties: props,
                body: message.Body
            );

            return Task.CompletedTask;
        }

        public Task Subscribe(Func<Message, Task<bool>> handler)
        {
            throw new InvalidOperationException("Exchange cannot be subscribed to");
        }

        public void Dispose()
        {
            this._channel?.Dispose();
            this._connection?.Dispose();
        }

        
        private void Create()
        {
            this._channel.ExchangeDeclare(
                exchange: this.Name,
                type: this.Type,
                durable: true,
                autoDelete: false,
                arguments: null
            );
        }

        private Task<bool> Handle(BasicDeliverEventArgs args, Func<Message, Task<bool>> handler)
        {
            if (!string.IsNullOrEmpty(args.BasicProperties.CorrelationId) &&
                !Guid.TryParse(args.BasicProperties.CorrelationId, out Guid correlationId)
            )
            {
                throw new InvalidDataException("Correlation ID should be a valid GUID");
            }

            var message = new Message(
                topic: args.RoutingKey,
                bodyType: args.BasicProperties.Type,
                body: args.Body,
                correlationId: correlationId,
                replyTo: args.BasicProperties.ReplyTo,
                headers: null
            );

            return handler(message);
        }
    }
}