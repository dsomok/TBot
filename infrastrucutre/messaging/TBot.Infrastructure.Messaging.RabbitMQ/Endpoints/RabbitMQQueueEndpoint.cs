using System;
using System.IO;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TBot.Infrastructure.Messaging.Abstractions;
using TBot.Infrastructure.Messaging.Abstractions.Endpoints;

namespace TBot.Infrastructure.Messaging.RabbitMQ.Endpoints
{
    class RabbitMQQueueEndpoint : IEndpoint
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;


        public RabbitMQQueueEndpoint(string name, ConnectionFactory connectionFactory)
        {
            Name = name;

            this._connection = connectionFactory.CreateConnection();
            this._channel = this._connection.CreateModel();
            this._consumer = new EventingBasicConsumer(this._channel);

            this.Create();
        }


        public string Name { get; }


        public void Bind(string exchangeName, string routingKey)
        {
            this._channel.QueueBind(
                queue: this.Name,
                exchange: exchangeName,
                routingKey: routingKey
            );
        }


        public Task Publish(Message message)
        {
            var props = this._channel.CreateBasicProperties();
            props.ReplyTo = message.ReplyTo;
            props.CorrelationId = message.CorrelationId.ToString();
            props.Type = message.BodyType;

            this._channel.BasicPublish(
                exchange: string.Empty,
                routingKey: this.Name,
                basicProperties: props,
                body: message.Body
            );

            return Task.CompletedTask;
        }

        public Task Subscribe(Func<Message, Task<bool>> handler)
        {
            this._consumer.Received += async (model, args) => await this.Handle(args, handler);

            this._channel.BasicConsume(
                queue: this.Name,
                autoAck: false,
                consumer: this._consumer
            );

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this._channel?.Dispose();
            this._connection?.Dispose();
        }


        private void Create()
        {
            this._channel.QueueDeclare(
                queue: this.Name,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        private async Task Handle(BasicDeliverEventArgs args, Func<Message, Task<bool>> handler)
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

            var handledSuccessfully = await handler(message);
            if (handledSuccessfully)
            {
                this._channel.BasicAck(args.DeliveryTag, false);
            }
        }
    }
}
