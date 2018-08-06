using System;
using TBot.Infrastructure.Hosting.Abstractions;
using TBot.Infrastructure.Messaging.Abstractions.Endpoints;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Topology
{
    public interface ITopology : IDisposable
    {
        string GetEventTopic<TEvent>() where TEvent : IEvent;
        string GetCommandTopic<TCommand>(string service) where TCommand : ICommand;
        string GetResponseTopic<TResponse>();

        IEndpoint ResolveEventSubscriptionEndpoint<TEvent>(string service) where TEvent : IEvent;
        IEndpoint ResolveEventPublishingEndpoint<TCommand>(Message message) where TCommand : IEvent;

        IEndpoint ResolveCommandSubscriptionEndpoint<TEvent>(string service) where TEvent : ICommand;
        IEndpoint ResolveCommandPublishingEndpoint<TCommand>(string service, Message message) where TCommand : ICommand;
        IEndpoint ResolveCommandReplyToEndpoint<TResponse>(HostContext hostContext) where TResponse : IMessage;
        IEndpoint ResolveCommandReplyToEndpoint(string replyTo);
    }
}
