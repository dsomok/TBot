using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    internal interface IMessageBuilder
    {
        Message Build<TMessage>(string topic, TMessage message) where TMessage : class, IMessage;
    }
}