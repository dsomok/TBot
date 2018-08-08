using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.Infrastructure.Bots.Messages
{
    public interface IBotCommand : ICommand
    {
        int ChatId { get; }

        int MessageId { get; }
    }
}
