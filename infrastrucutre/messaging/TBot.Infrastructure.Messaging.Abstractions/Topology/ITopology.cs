using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Topology
{
    public interface ITopology
    {
        IEndpoint ResolveEventSubscriptionEndpoint<TMessage>(string service) where TMessage : IEvent;
        IEndpoint ResolveCommandSubscriptionEndpoint<TMessage>(string service) where TMessage : ICommand;

        IEndpoint ResolveEventPublishingEndpoint<TMessage>(Message message) where TMessage : IEvent;
        IEndpoint ResolveCommandPublishingEndpoint<TMessage>(string service, Message message) where TMessage : ICommand;
    }
}
