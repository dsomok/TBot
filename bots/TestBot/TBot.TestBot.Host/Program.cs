using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using TBot.Infrastructure.Hosting.Console;

namespace TBot.TestBot.Host
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = ConsoleHostBuilder
                       .Create()
                       .WithConfiguration(configurationBuilder =>
                           {
                               configurationBuilder.AddJsonFile("appsettings.json");
                           })
                       .WithLogger(loggerConfiguration => loggerConfiguration.WriteTo.Console())
                       .Build();

            await host.Run();
        }
    }
}
