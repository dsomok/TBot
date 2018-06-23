using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace TBot.Infrastructure.Hosting.Abstractions
{
    public interface IHost
    {
        Task Run();
        Task Run(Func<IContainer, IConfiguration, Task> action);
    }
}
