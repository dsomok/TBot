using System;
using System.Threading.Tasks;

namespace TBot.Infrastructure.Messaging.Abstractions.Endpoints
{
    public interface IEndpoint : IDisposable
    {
        string Name { get; }

        bool IsSubscribed { get; }
        
        Task Publish(Message message);
        Task<Guid> Subscribe(Func<Message, Task<bool>> handler);
        Task Unsubscribe(Guid subscriptionId);
    }
}