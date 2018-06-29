using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    public interface ICommandBus
    {
        // Task<ISubscription> RegisterHandler<TCommand>(string service, Func<TCommand, Task> handler) where TCommand : ICommand;

        Task Send<TCommand>(string service, TCommand command) where TCommand : ICommand;
    }

    public class Message
    {
        public Message(string topic, byte[] body)
            : this(topic, body, Guid.NewGuid(), string.Empty, new Dictionary<string, string>())
        {
        }

        public Message(
            string topic,
            byte[] body,
            Guid correlationId, 
            string replyTo, 
            IDictionary<string, string> headers
        )
        {
            Topic = topic;
            Body = body;
            CorrelationId = correlationId;
            ReplyTo = replyTo;
            Headers = headers;
        }


        public Guid CorrelationId { get; }

        public string ReplyTo { get; }

        public string Topic { get; }

        public IDictionary<string, string> Headers { get; }

        public byte[] Body { get; }
    }

    public interface ISubscriber
    {
        Task Subscribe<TMessage>(Func<Message, Task> handler) where TMessage : IMessage;
    }

    public interface IPublisher
    {
        Task Publish(Message message);
    }

    class EventBus : IEventBus
    {
        public Task<ISubscription> Subscribe<TEvent>() where TEvent : IEvent
        {
            throw new NotImplementedException();
        }

        public Task Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            throw new NotImplementedException();
        }
    }

    class CommandBus : ICommandBus
    {
        private readonly IPublisher _publisher;
        private readonly ISerializer _serializer;


        public Task Send<TCommand>(string service, TCommand command) where TCommand : ICommand
        {
            var topic = $"{service}.{typeof(TCommand).Name}";
            var body = this._serializer.SerializeAsBytes(command);

            var message = new Message(topic, body);

            return this._publisher.Publish(message);
        }
    }
}