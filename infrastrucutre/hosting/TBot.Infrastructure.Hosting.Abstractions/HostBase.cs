using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TBot.Infrastructure.Hosting.Abstractions
{
    public abstract class HostBase : IHost
    {
        protected readonly IList<Func<IContainer, IConfiguration, Task>> OnStartActions;
        protected readonly IList<Func<IContainer, IConfiguration, Task>> OnStopActions;

        protected readonly IContainer Container;
        protected readonly IConfiguration Configuration;

        protected readonly ILogger Logger;

        protected HostBase(
            IList<Func<IContainer, IConfiguration, Task>> onStartActions,
            IList<Func<IContainer, IConfiguration, Task>> onStopActions,
            IContainer container,
            IConfiguration configuration
        )
        {
            OnStartActions = onStartActions;
            OnStopActions = onStopActions;
            Container = container;
            Configuration = configuration;
            Logger = container.Resolve<ILogger>();
        }


        public virtual Task Run()
        {
            return this.Run((container, config) => Task.CompletedTask);
        }

        public virtual async Task Run(Func<IContainer, IConfiguration, Task> action)
        {
            var typeName = this.GetType().Name;

            await this.RunOnStartActions();

            this.Logger.Information($"{typeName} has started");
            await action(this.Container, this.Configuration);

            await this.RunHost();

            await this.RunOnStopActions();
            this.Logger.Information($"{typeName} has stopped");
        }


        protected abstract Task RunHost();


        private async Task RunOnStartActions()
        {
            this.Logger.Information("Running OnStart actions");
            foreach (var onStartAction in this.OnStartActions)
            {
                await onStartAction(this.Container, this.Configuration);
            }
        }

        private async Task RunOnStopActions()
        {
            this.Logger.Information("Running OnStop actions");
            foreach (var onStopAction in this.OnStopActions)
            {
                await onStopAction(this.Container, this.Configuration);
            }
        }
    }
}
