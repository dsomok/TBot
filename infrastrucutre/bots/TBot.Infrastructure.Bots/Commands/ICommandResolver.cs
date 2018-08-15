using TBot.Infrastructure.Bots.Contracts.API.Update;
using TBot.Infrastructure.Bots.Contracts.Messages;

namespace TBot.Infrastructure.Bots.Commands
{
    public interface ICommandResolver
    {
        IBotCommand CreateCommand(UpdateModel updateModel);
    }
}