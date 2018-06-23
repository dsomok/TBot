using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TBot.Infrastructure.Hosting.Abstractions;

namespace TBot.Infrastructure.Hosting.AspNetCore
{
    internal class AspNetHost : HostBase
    {
        private readonly IWebHost _webHost;


        public AspNetHost(
            IList<Func<IContainer, IConfiguration, Task>> onStartActions,
            IList<Func<IContainer, IConfiguration, Task>> onStopActions,
            IContainer container,
            IConfiguration configuration,
            IWebHost webHost
        ) : base(onStartActions, onStopActions, container, configuration)
        {
            _webHost = webHost;
        }

        protected override Task RunHost()
        {
            return this._webHost?.RunAsync();
        }
    }
}