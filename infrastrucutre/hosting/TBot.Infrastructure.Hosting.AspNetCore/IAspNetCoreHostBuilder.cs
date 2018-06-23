using System;
using Microsoft.AspNetCore.Hosting;
using TBot.Infrastructure.Hosting.Abstractions;

namespace TBot.Infrastructure.Hosting.AspNetCore
{
    public interface IAspNetCoreHostBuilder : IHostBuilder<IAspNetCoreHostBuilder>
    {
        IAspNetCoreHostBuilder WithWebHost(Func<IWebHostBuilder> builderFactory);
    }
}
