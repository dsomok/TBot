using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using TBot.Infrastructure.Messaging.Abstractions.Handlers;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Messaging.Abstractions.Hosting
{
    class RegistrationsBuilder
    {
        private readonly string _service;
        private readonly Assembly[] _assemblies;


        public RegistrationsBuilder(string serviceName, Assembly[] assemblies)
        {
            this._service = serviceName;
            this._assemblies = assemblies;
        }


        public List<(Type HandlerType, Type MessageType)> GetHandlers(Type genericHandlerType)
        {
            return (
                from assembly in this._assemblies
                from type in assembly.DefinedTypes
                from implementedInterface in type.ImplementedInterfaces
                where implementedInterface.IsGenericType &&
                      implementedInterface.GetGenericTypeDefinition() == genericHandlerType
                let commandType = implementedInterface.GenericTypeArguments.First()
                select (HandlerType: type.UnderlyingSystemType, MessageType: commandType)
            ).ToList();
        }

        public async Task RegisterCommandHandlers(IContainer container)
        {
            var commandHandlers = this.GetHandlers(typeof(ICommandHandler<>));
            foreach (var handler in commandHandlers)
            {
                var builder = new RegistrationProcessBuilder<ICommandBus>(handler, container);
                var registerProcess = builder.Build(
                    serviceName: this._service,
                    handleMethodName: nameof(ICommandHandler<ICommand>.Handle),
                    messageBusRegistrationMethodName: nameof(ICommandBus.RegisterHandler)
                );

                await registerProcess;
            }
        }

        public async Task RegisterEventHandlers(IContainer container)
        {
            var eventHandlers = this.GetHandlers(typeof(IEventHandler<>));
            foreach (var handler in eventHandlers)
            {
                var builder = new RegistrationProcessBuilder<IEventBus>(handler, container);
                var registerProcess = builder.Build(
                    serviceName: this._service,
                    handleMethodName: nameof(IEventHandler<IEvent>.Handle),
                    messageBusRegistrationMethodName: nameof(IEventBus.Subscribe)
                );

                await registerProcess;
            }
        }
    }
}
