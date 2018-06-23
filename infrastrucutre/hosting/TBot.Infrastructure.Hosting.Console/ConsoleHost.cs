using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using TBot.Infrastructure.Hosting.Abstractions;

namespace TBot.Infrastructure.Hosting.Console
{
    internal class ConsoleHost : HostBase
    {
        public ConsoleHost(
            IList<Func<IContainer, IConfiguration, Task>> onStartActions,
            IList<Func<IContainer, IConfiguration, Task>> onStopActions,
            IContainer container,
            IConfiguration configuration
        ) : base(onStartActions, onStopActions, container, configuration)
        {
        }

        protected override Task RunHost()
        {
            System.Console.ReadKey();
            return Task.CompletedTask;
        }
    }
}
