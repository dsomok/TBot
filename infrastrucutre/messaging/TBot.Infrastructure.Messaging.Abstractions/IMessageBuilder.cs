using System;
using TBot.Infrastructure.Messaging.Abstractions.Endpoints;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    internal interface IMessageBuilder
    {
        Message Build<TMessage>(string topic, TMessage message) where TMessage : class, IMessage;
        Message Build<TMessage>(string topic, TMessage message, IEndpoint replyToEndpoint) where TMessage : class, IMessage;

        Message Build<TMessage>(string topic, Guid correlationId, TMessage message)
            where TMessage : class, IMessage;
    }
}