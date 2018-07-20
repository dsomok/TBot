using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using TBot.Infrastructure.Bots;
using TBot.Infrastructure.Hosting.Console;
using TBot.Infrastructure.Messaging.RabbitMQ;

namespace TBot.GLObot.Host
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceName = "GLObot";
            var host = ConsoleHostBuilder
                       .Create()
                       .WithName(serviceName)
                       .WithConfiguration(configurationBuilder =>
                       {
                           configurationBuilder.AddJsonFile("appsettings.json");
                       })
                       .WithLogger(loggerConfiguration => loggerConfiguration.WriteTo.Console())
                       //.WithMessaging(serviceName, typeof(StartCommandHandler).Assembly)
                       .WithRabbitMQMessaging(config => new RabbitMQMessagingSettings(
                           hostName: config["RabbitMQ:HostName"],
                           userName: config["RabbitMQ:UserName"],
                           password: config["RabbitMQ:Password"])
                       )
                       .WithTelegramBot(config => new TelegramBotSettings(
                           name: config["botName"],
                           token: config["botToken"])
                       )
                       .Build();

            await host.Run();
        }
    }
}
