using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TBot.Infrastructure.Hosting.Abstractions
{
    public abstract class HostBuilderBase<TBuilder> : IHostBuilder<TBuilder> 
        where TBuilder : IHostBuilder<TBuilder>
    {
        protected readonly ContainerBuilder ContainerBuilder = new ContainerBuilder();
        protected IConfiguration Configuration = new ConfigurationRoot(new List<IConfigurationProvider>());

        protected readonly IList<Func<IContainer, IConfiguration, Task>> OnStartActions =
            new List<Func<IContainer, IConfiguration, Task>>();

        protected readonly IList<Func<IContainer, IConfiguration, Task>> OnStopActions =
            new List<Func<IContainer, IConfiguration, Task>>();


        protected abstract TBuilder Builder { get; }


        public TBuilder AsService(string serviceName)
        {
            var hostContext = new HostContext(serviceName);
            this.ContainerBuilder.RegisterInstance(hostContext).AsSelf();

            return this.Builder;
        }

        public TBuilder WithConfiguration(Action<IConfigurationBuilder> configurator)
        {
            var builder = new ConfigurationBuilder();
            configurator(builder);

            this.Configuration = builder.Build();

            this.ContainerBuilder.RegisterInstance(this.Configuration).AsImplementedInterfaces().SingleInstance();

            return this.Builder;
        }

        public TBuilder WithLogger(Func<LoggerConfiguration, LoggerConfiguration> loggerConfigurator)
        {
            var config = new LoggerConfiguration();
            config = loggerConfigurator(config);

            var logger = config.CreateLogger();
            this.ContainerBuilder.RegisterInstance(logger).AsImplementedInterfaces().SingleInstance();

            return this.Builder;
        }

        public TBuilder WithServices(Action<ContainerBuilder> serviceConfigurator)
        {
            serviceConfigurator(this.ContainerBuilder);
            return this.Builder;
        }

        public TBuilder WithServices(Action<ContainerBuilder, IConfiguration> serviceConfigurator)
        {
            serviceConfigurator(this.ContainerBuilder, this.Configuration);
            return this.Builder;
        }

        public TBuilder OnStart(Func<IContainer, IConfiguration, Task> action)
        {
            this.OnStartActions.Add(action);
            return this.Builder;
        }

        public TBuilder OnStop(Func<IContainer, IConfiguration, Task> action)
        {
            this.OnStopActions.Add(action);
            return this.Builder;
        }


        public abstract IHost Build();
    }
}
