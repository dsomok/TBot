using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace TBot.Infrastructure.Hosting.AspNetCore
{
    internal class ServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private readonly IContainer _container;

        public ServiceProviderFactory(IContainer container)
        {
            _container = container;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            var aspNetScope = this._container.BeginLifetimeScope(containerBuilder =>
            {
                containerBuilder.Populate(services);
            });

            return new AutofacServiceProvider(aspNetScope);
        }
    }
}