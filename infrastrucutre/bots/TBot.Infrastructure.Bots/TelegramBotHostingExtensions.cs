using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using TBot.Infrastructure.Bots.API;
using TBot.Infrastructure.Bots.Commands;
using TBot.Infrastructure.Bots.Contracts.Commands;
using TBot.Infrastructure.Bots.Handlers;
using TBot.Infrastructure.Bots.Handlers.Context;
using TBot.Infrastructure.Bots.HttpClient;
using TBot.Infrastructure.Hosting.Abstractions;
using TBot.Infrastructure.Messaging.Abstractions;

namespace TBot.Infrastructure.Bots
{
    public static class TelegramBotHostingExtensions
    {

        public static IHostBuilder<TBuilder> WithTelegramBot<TBuilder, TCommandResolver>(
            this IHostBuilder<TBuilder> hostBuilder, 
            Func<IConfiguration, TelegramBotSettings> settings
        ) 
            where TBuilder: IHostBuilder<TBuilder>
            where TCommandResolver: class, ICommandResolver
        {
            return hostBuilder
                   .WithServices((builder, config) =>
                   {
                       var botSettings = settings(config);

                       builder.Register(context =>
                       {
                           var telegramApi = context.Resolve<ITelegramApi>();
                           return new BotHandlerContextFactory(botSettings.Token, telegramApi);
                       }).AsImplementedInterfaces().SingleInstance();

                       builder.RegisterType<TelegramApi>().AsImplementedInterfaces().SingleInstance();
                       builder.RegisterType<TelegramHttpClient>().AsImplementedInterfaces().SingleInstance();
                       builder.RegisterType<NewUpdateCommandHandler>().AsSelf().AsImplementedInterfaces().SingleInstance();
                       builder.RegisterType<TCommandResolver>().AsImplementedInterfaces().SingleInstance();
                   })
                   .OnStart(async (container, config) =>
                   {
                       var botSettings = settings(config);

                       var logger = container.Resolve<ILogger>();
                       var telegramApi = container.Resolve<ITelegramApi>();

                       if (await telegramApi.DeleteWebhook(botSettings.Token))
                       {
                           logger.Information("Deleted webhook for bot {Bot}", botSettings.Name);
                       }

                       var webhookUrl = $"{botSettings.ApiURL}/{botSettings.Name}";
                       if (await telegramApi.SetWebhook(botSettings.Token, webhookUrl))
                       {
                           logger.Information("Set webhook for bot {Bot}: {BotWebhook}", botSettings.Name, webhookUrl);
                       }

                       await RegisterNewUpdateCommandHandler(container);
                   });
        }

        private static Task RegisterNewUpdateCommandHandler(IContainer container)
        {
            var hostContext = container.Resolve<HostContext>();
            var commandBus = container.Resolve<ICommandBus>();
            var handler = container.Resolve<NewUpdateCommandHandler>();
            return commandBus.RegisterHandler<NewUpdateCommand>(hostContext.ServiceName, handler.Handle);
        }
    }
}
