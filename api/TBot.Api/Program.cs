using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using TBot.Infrastructure.Hosting.AspNetCore;

namespace TBot.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = AspNetCoreHostBuilder
                       .Create()
                       .WithConfiguration(configurationBuilder => { })
                       .WithLogger(loggerConfiguration => loggerConfiguration.WriteTo.Console())
                       .WithWebHost(() => BuildWebHost(args))
                       .Build();

            await host.Run();
        }

        public static IWebHostBuilder BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>();
    }
}
