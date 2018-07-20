namespace TBot.Infrastructure.Hosting.Abstractions
{
    public class HostContext
    {
        public HostContext(string serviceName)
        {
            ServiceName = serviceName;
        }

        public string ServiceName { get; }
    }
}
