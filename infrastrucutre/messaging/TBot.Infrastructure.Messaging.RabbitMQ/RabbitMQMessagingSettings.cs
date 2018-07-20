namespace TBot.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQMessagingSettings
    {
        public RabbitMQMessagingSettings(string hostName, string userName, string password)
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
        }

        public string HostName { get; }
        public string UserName { get; }
        public string Password { get; }
    }
}
