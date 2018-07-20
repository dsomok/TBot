using Autofac;

namespace TBot.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonSerializer>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
