using Autofac;
using TBot.Infrastructure.Messaging.Abstractions.Endpoints;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    public class MessagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<InfrastructureModule>();

            builder.RegisterType<EndpointRegistry>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SubscriptionsRegistry>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CommandBus>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<EventBus>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<MessageBuilder>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
