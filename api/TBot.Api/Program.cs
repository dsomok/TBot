using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using TBot.Infrastructure.Hosting.AspNetCore;
using TBot.Infrastructure.Messaging.Abstractions.Hosting;
using TBot.Infrastructure.Messaging.RabbitMQ;

namespace TBot.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceName = "TBot.Api";
            var host = AspNetCoreHostBuilder
                       .Create()
                       .AsService(serviceName)
                       .WithConfiguration(configurationBuilder => {
                           configurationBuilder.AddJsonFile("appsettings.json");
                       })
                       .WithLogger(loggerConfiguration => loggerConfiguration.WriteTo.Console())
                       .WithMessaging(serviceName)
                       .WithRabbitMQMessaging(config => new RabbitMQMessagingSettings(
                           hostName: config["RabbitMQ:HostName"],
                           userName: config["RabbitMQ:UserName"],
                           password: config["RabbitMQ:Password"])
                       )
                       .WithWebHost(() => BuildWebHost(args))
                       .Build();

            await host.Run();
        }

        public static IWebHostBuilder BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>();
    }
}
