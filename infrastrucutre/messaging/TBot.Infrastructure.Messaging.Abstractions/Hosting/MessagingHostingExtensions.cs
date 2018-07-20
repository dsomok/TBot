using System.Reflection;
using Autofac;
using TBot.Infrastructure.Hosting.Abstractions;
using TBot.Infrastructure.Messaging.Abstractions.Handlers;

namespace TBot.Infrastructure.Messaging.Abstractions.Hosting
{

    public static class MessagingHostingExtensions
    {
        public static IHostBuilder<TBuilder> WithMessaging<TBuilder>(this IHostBuilder<TBuilder> hostBuilder, string service, params Assembly[] handlersAssemblies)
            where TBuilder: IHostBuilder<TBuilder>
        {
            return hostBuilder
                   .WithServices(services =>
                   {
                       var registrationsBuilder = new RegistrationsBuilder(service, handlersAssemblies);

                       services.RegisterInstance(registrationsBuilder);
                       services.RegisterModule<MessagingModule>();
                       
                       var commandHandlers = registrationsBuilder.GetHandlers(typeof(ICommandHandler<>));
                       foreach (var handler in commandHandlers)
                       {
                           var iCommandHandlerType = typeof(ICommandHandler<>).MakeGenericType(handler.MessageType);
                           services.RegisterType(handler.HandlerType).As(iCommandHandlerType).AsSelf();
                       }

                       var eventHandlers = registrationsBuilder.GetHandlers(typeof(IEventHandler<>));
                       foreach (var handler in eventHandlers)
                       {
                           var iCommandHandlerType = typeof(IEventHandler<>).MakeGenericType(handler.MessageType);
                           services.RegisterType(handler.HandlerType).As(iCommandHandlerType).AsSelf();
                       }
                   })
                   .OnStart(async (container, config) =>
                   {
                       var registrationsBuilder = container.Resolve<RegistrationsBuilder>();

                       await registrationsBuilder.RegisterCommandHandlers(container);
                       await registrationsBuilder.RegisterEventHandlers(container);
                   });
        }
    }
}
