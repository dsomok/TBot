using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using TBot.Infrastructure.Bots.API;
using TBot.Infrastructure.Bots.Handlers.Context;
using TBot.Infrastructure.Bots.HttpClient;
using TBot.Infrastructure.Hosting.Abstractions;

namespace TBot.Infrastructure.Bots
{
    public static class TelegramBotHostingExtensions
    {

        public static IHostBuilder<TBuilder> WithTelegramBot<TBuilder>(this IHostBuilder<TBuilder> hostBuilder, Func<IConfiguration, TelegramBotSettings> settings) 
            where TBuilder: IHostBuilder<TBuilder>
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
                   });
        }
    }
}
