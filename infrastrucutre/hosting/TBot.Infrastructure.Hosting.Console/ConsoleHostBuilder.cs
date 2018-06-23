using TBot.Infrastructure.Hosting.Abstractions;

namespace TBot.Infrastructure.Hosting.Console
{
    public class ConsoleHostBuilder : HostBuilderBase<IConsoleHostBuilder>, IConsoleHostBuilder
    {
        private ConsoleHostBuilder()
        {
        }


        public static IConsoleHostBuilder Create()
        {
            return new ConsoleHostBuilder();
        }


        protected override IConsoleHostBuilder Builder => this;


        public override IHost Build()
        {
            var container = this.ContainerBuilder.Build();
            return new ConsoleHost(this.OnStartActions, this.OnStopActions, container, this.Configuration);
        }
    }
}
