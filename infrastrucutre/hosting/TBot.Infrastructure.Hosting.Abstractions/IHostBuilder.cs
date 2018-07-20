using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TBot.Infrastructure.Hosting.Abstractions
{
    public interface IHostBuilder<TBuilder> 
        where TBuilder : IHostBuilder<TBuilder>
    {
        TBuilder AsService(string serviceName);
        TBuilder WithConfiguration(Action<IConfigurationBuilder> configuration);
        TBuilder WithLogger(Func<LoggerConfiguration, LoggerConfiguration> configuration);
        TBuilder WithServices(Action<ContainerBuilder> configuration);
        TBuilder WithServices(Action<ContainerBuilder, IConfiguration> configuration);

        TBuilder OnStart(Func<IContainer, IConfiguration, Task> action);
        TBuilder OnStop(Func<IContainer, IConfiguration, Task> action);

        IHost Build();
    }
}