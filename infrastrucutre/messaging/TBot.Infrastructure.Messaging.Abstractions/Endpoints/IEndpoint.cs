﻿using System;
using System.Threading.Tasks;

namespace TBot.Infrastructure.Messaging.Abstractions.Endpoints
{
    public interface IEndpoint : IDisposable
    {
        string Name { get; }
        
        Task Publish(Message message);
        Task Subscribe(Func<Message, Task<bool>> handler);
    }
}