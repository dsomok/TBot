using System;
using System.Collections.Generic;
using TBot.Infrastructure.Messaging.Abstractions.Endpoints;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    internal class MessageBuilder : IMessageBuilder
    {
        private readonly ISerializer _serializer;


        public MessageBuilder(ISerializer serializer)
        {
            _serializer = serializer;
        }


        public Message Build<TMessage>(string topic, TMessage message) 
            where TMessage : class, IMessage
        {
            var bodyType = typeof(TMessage).FullName;
            var body = this._serializer.SerializeAsBytes(message);

            return new Message(topic, bodyType, body);
        }

        public Message Build<TMessage>(string topic, Guid correlationId, TMessage message)
            where TMessage : class, IMessage
        {
            var bodyType = typeof(TMessage).FullName;
            var body = this._serializer.SerializeAsBytes(message);

            return new Message(topic, bodyType, body, correlationId);
        }

        public Message Build<TMessage>(string topic, TMessage message, IEndpoint replyToEndpoint) 
            where TMessage : class, IMessage
        {
            var bodyType = typeof(TMessage).FullName;
            var body = this._serializer.SerializeAsBytes(message);
            var correlationId = Guid.NewGuid();
            var replyTo = replyToEndpoint.Name;

            return new Message(
                topic: topic,
                bodyType: bodyType,
                body: body,
                correlationId: correlationId,
                replyTo: replyTo,
                headers: new Dictionary<string, string>()
            );
        }
    }
}