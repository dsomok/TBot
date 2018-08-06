using System;
using System.Collections.Generic;
using System.Text;

namespace TBot.Infrastructure.Messaging.Abstractions.Exceptions
{
    public class EndpointNotFoundException : Exception
    {
        public EndpointNotFoundException(string endpointName)
        {
            EndpointName = endpointName;
        }

        public string EndpointName { get; }

        public override string Message => $"Endpoint \"{this.EndpointName}\" was not found";
    }
}
