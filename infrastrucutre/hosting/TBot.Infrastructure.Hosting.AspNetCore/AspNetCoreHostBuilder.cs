using System;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TBot.Infrastructure.Hosting.Abstractions;

namespace TBot.Infrastructure.Hosting.AspNetCore
{
    public class AspNetCoreHostBuilder : HostBuilderBase<IAspNetCoreHostBuilder>, IAspNetCoreHostBuilder
    {
        private AspNetCoreHostBuilder()
        {
        }

        public static IAspNetCoreHostBuilder Create()
        {
            return new AspNetCoreHostBuilder();
        }

        protected override IAspNetCoreHostBuilder Builder => this;


        public IAspNetCoreHostBuilder WithWebHost(Func<IWebHostBuilder> builderFactory)
        {
            return this.WithServices(services =>
            {
                var webHostBuilder = builderFactory();
                services.RegisterInstance(webHostBuilder).AsImplementedInterfaces().SingleInstance();
            });
            //return this.OnStart((container, config) =>
            //{
            //    var builder = builderFactory();
            //    builder.ConfigureServices(services =>
            //    {
            //        var providerFactory = new ServiceProviderFactory(container);
            //        services.AddTransient<IServiceProviderFactory<IServiceCollection>, ServiceProviderFactory>(
            //            _ => providerFactory
            //        );
            //    });

            //    this._webHost = builder.Build();
            //    return Task.CompletedTask;
            //});
        }


        public override IHost Build()
        {
            var container = this.ContainerBuilder.Build();

            var webHostBuilder = container.Resolve<IWebHostBuilder>();
            webHostBuilder.ConfigureServices(services =>
            {
                var providerFactory = new ServiceProviderFactory(container);
                services.AddSingleton<IServiceProviderFactory<IServiceCollection>, ServiceProviderFactory>(_ => providerFactory);
            });

            var webHost = webHostBuilder.Build();

            return new AspNetHost(this.OnStartActions, this.OnStopActions, container, this.Configuration, webHost);
        }
    }
}