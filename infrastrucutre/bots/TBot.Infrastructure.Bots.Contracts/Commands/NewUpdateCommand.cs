using TBot.Infrastructure.Bots.Contracts.API.Update;
using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Bots.Contracts.Commands
{
    public class NewUpdateCommand : ICommand
    {
        public NewUpdateCommand(UpdateModel update)
        {
            Update = update;
        }

        public UpdateModel Update { get; }
    }
}
