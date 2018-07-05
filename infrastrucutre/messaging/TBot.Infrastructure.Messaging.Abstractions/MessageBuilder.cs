using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    internal class MessageBuilder : IMessageBuilder
    {
        private readonly ISerializer _serializer;

        public Message Build<TMessage>(string topic, TMessage message) 
            where TMessage : class, IMessage
        {
            var bodyType = typeof(TMessage).FullName;
            var body = this._serializer.SerializeAsBytes(message);

            return new Message(topic, bodyType, body);
        }
    }
}