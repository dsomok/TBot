using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using TBot.Infrastructure.Hosting.Abstractions;
using TBot.Infrastructure.Messaging.RabbitMQ.Topology;

namespace TBot.Infrastructure.Messaging.RabbitMQ
{
    public static class RabbitMQMessagingHostingExtensions
    {
        public static TBuilder WithRabbitMQMessaging<TBuilder>(this IHostBuilder<TBuilder> builder, Func<IConfiguration, RabbitMQMessagingSettings> settings)
            where TBuilder : IHostBuilder<TBuilder>
        {
            return builder.WithServices((services, config) =>
            {
                var rabbitMQSettings = settings(config);

                services.RegisterType<SimpleRabbitMQTopology>().AsImplementedInterfaces().SingleInstance();
                
                services.RegisterInstance(new ConnectionFactory
                {
                    HostName = rabbitMQSettings.HostName,
                    UserName = rabbitMQSettings.UserName,
                    Password = rabbitMQSettings.Password
                });
            });
        }
    }
}
