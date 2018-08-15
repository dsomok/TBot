using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Serilog;
using TBot.Infrastructure.Bots.Commands;
using TBot.Infrastructure.Bots.Contracts.Commands;
using TBot.Infrastructure.Bots.Contracts.Exceptions;
using TBot.Infrastructure.Bots.Contracts.Messages;
using TBot.Infrastructure.Messaging.Abstractions.Handlers;

namespace TBot.Infrastructure.Bots.Handlers
{
    public class NewUpdateCommandHandler : ICommandHandler<NewUpdateCommand>
    {
        private readonly IComponentContext _container;
        private readonly ICommandResolver _commandResolver;
        private readonly ILogger _logger;


        public NewUpdateCommandHandler(IComponentContext container, ICommandResolver commandResolver, ILogger logger)
        {
            _container = container;
            _commandResolver = commandResolver;
            _logger = logger;
        }


        public Task Handle(NewUpdateCommand update)
        {
            var command = this._commandResolver.CreateCommand(update.Update);

            this._logger.Information("New update was received with command {CommandType}", command);

            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

            if (!this._container.TryResolve(handlerType, out var handler))
            {
                throw new BotCommandHandlerNotFound(commandType.Name);
            }

            var handleMethod = handlerType
                               .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                               .SingleOrDefault(m =>
                                   m.Name == nameof(ICommandHandler<IBotCommand>.Handle) &&
                                   m.GetParameters().Length == 1
                               );

            return (Task) handleMethod.Invoke(handler, new[] {command});
        }
    }
}
