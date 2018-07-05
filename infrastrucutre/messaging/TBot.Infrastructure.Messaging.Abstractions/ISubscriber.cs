using System;
using System.Threading.Tasks;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    public interface ISubscriber : IDisposable
    {
        Task Subscribe(Func<Message, Task<bool>> handler);
    }
}