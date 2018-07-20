using System;

namespace TBot.Infrastructure.Messaging.Abstractions.Endpoints
{
    public interface IEndpointRegistry : IDisposable
    {
        void Add(IEndpoint endpoint);

        bool TryGet(string name, out IEndpoint endpoint);

        bool TryGet<TEndpoint>(string name, out TEndpoint endpoint) where TEndpoint : class, IEndpoint;

        IEndpoint this[string name] { get; set; }
    }
}