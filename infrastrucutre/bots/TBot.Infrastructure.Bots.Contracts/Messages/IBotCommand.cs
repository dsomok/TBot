using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Bots.Contracts.Messages
{
    public interface IBotCommand : ICommand
    {
        int ChatId { get; }

        int MessageId { get; }
    }
}
