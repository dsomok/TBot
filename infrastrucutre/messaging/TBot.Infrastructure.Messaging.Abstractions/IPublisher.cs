using System;
using System.Threading.Tasks;

namespace TBot.Infrastructure.Messaging.Abstractions
{
    public interface IPublisher : IDisposable
    {
        Task Publish(Message message);
    }
}