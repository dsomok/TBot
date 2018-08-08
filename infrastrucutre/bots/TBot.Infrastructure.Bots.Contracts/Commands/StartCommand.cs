using TBot.Infrastructure.Bots.Messages;

namespace TBot.Infrastructure.Bots.Contracts.Commands
{
    public class StartCommand : IBotCommand
    {
        public StartCommand(int chatId, int messageId)
        {
            ChatId = chatId;
            MessageId = messageId;
        }

        public int ChatId { get; }
        public int MessageId { get; }
    }
}
