using System;
using System.Threading.Tasks;
using Serilog;
using TBot.Infrastructure.Messaging.Abstractions.Endpoints;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Subscriptions
{
    class Subscription<TMessage> : ISubscription where TMessage : class, IMessage
    {
        private readonly ISerializer _serializer;
        private readonly ISubscriptionsRegistry _registry;
        private readonly ILogger _logger;
        private readonly Func<IMessage, Message, Task<bool>> _handler;


        public Subscription(
            ISubscriptionsRegistry registry,
            ISerializer serializer,
            ILogger logger, 
            IEndpoint endpoint, 
            Func<TMessage, Message, Task<bool>> handler
        )
        {
            Id = Guid.NewGuid();
            _registry = registry;
            _serializer = serializer;
            _logger = logger;
            Endpoint = endpoint;
            _handler = (iMessage, message) => this.Handle(iMessage, message, handler);
        }


        public Guid Id { get; }

        public IEndpoint Endpoint { get; }

        public Type Type => typeof(TMessage);

        
        public Task<bool> Handle(Message message)
        {
            var messageType = message.BodyType;
            var iMessage = this._serializer.Deserialize(message.Body) as IMessage;
            if (iMessage == null)
            {
                this._logger.Warning("Failed to deserialize message of type {MessageType}", messageType);
                throw new InvalidCastException($"Failed to deserialize message of type {messageType}");
            }

            return this._handler.Invoke(iMessage, message);
        }
        

        private Task<bool> Handle(IMessage iMessage, Message message, Func<TMessage, Message, Task<bool>> handler)
        {
            if (!(iMessage is TMessage tMessage))
            {
                throw new InvalidCastException($"This subscription is not intended to handle messages of type {message.GetType().Name}");
            }

            return handler(tMessage, message);
        }
        

        public void Dispose()
        {
            this._registry.DeleteSubscription(this);
        }
    }
}