using System;
using System.Collections.Generic;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    public class Message
    {
        public Message(string topic, string bodyType, byte[] body)
            : this(topic, bodyType, body, Guid.NewGuid(), string.Empty, new Dictionary<string, string>())
        {
        }

        public Message(
            string topic, 
            string bodyType,
            byte[] body,
            Guid correlationId, 
            string replyTo, 
            IDictionary<string, string> headers
        )
        {
            Topic = topic;
            BodyType = bodyType;
            Body = body;
            CorrelationId = correlationId;
            ReplyTo = replyTo;
            Headers = headers;
        }


        public Guid CorrelationId { get; }

        public string ReplyTo { get; }

        public string Topic { get; }

        public string BodyType { get; }

        public IDictionary<string, string> Headers { get; }

        public byte[] Body { get; }
    }
}