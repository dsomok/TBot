using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Infrastructure.Messaging.Abstractions.Topology
{
    public interface ITopology
    {
        Task SendCommand(Message command);

        Task SubscribeForCommands(string service);
    }
}
