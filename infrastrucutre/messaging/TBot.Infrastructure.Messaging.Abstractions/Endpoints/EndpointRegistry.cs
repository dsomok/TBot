using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TBot.Infrastructure.Messaging.Abstractions.Endpoints
{
    class EndpointRegistry : IEndpointRegistry
    {
        private readonly ConcurrentDictionary<string, IEndpoint> _endpoints = new ConcurrentDictionary<string, IEndpoint>();

        public void Add(IEndpoint endpoint)
        {
            this._endpoints.AddOrUpdate(
                key: endpoint.Name,
                addValue: endpoint,
                updateValueFactory: (name, existingEndpoint) => endpoint
            );
        }

        public bool TryGet(string name, out IEndpoint endpoint)
        {
            return this._endpoints.TryGetValue(name, out endpoint);
        }

        public bool TryGet<TEndpoint>(string name, out TEndpoint endpoint) where TEndpoint : class, IEndpoint
        {
            if (!this._endpoints.TryGetValue(name, out IEndpoint e))
            {
                endpoint = null;
                return false;
            }

            endpoint = e as TEndpoint;
            return endpoint != null;
        }

        public IEndpoint this[string name]
        {
            get
            {
                if (!this.TryGet(name, out IEndpoint endpoint))
                {
                    throw new KeyNotFoundException();
                }

                return endpoint;
            }
            set => this.Add(value);
        }

        public void Dispose()
        {
            foreach (var endpoint in this._endpoints.Values)
            {
                endpoint.Dispose();
            }
        }
    }
}