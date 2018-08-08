using System.Threading.Tasks;
using TBot.Infrastructure.Bots.API;

namespace TBot.Infrastructure.Bots.Handlers.Context
{
    public class BotHandlerContext : IBotHandlerContext
    {
        private readonly string _token;
        private readonly ITelegramApi _telegramApi;

        public BotHandlerContext(int messageId, int chatId, string token, ITelegramApi telegramApi)
        {
            MessageId = messageId;
            ChatId = chatId;
            _token = token;
            _telegramApi = telegramApi;
        }

        public int MessageId { get; }
        public int ChatId { get; }


        public Task SendMessage(string message)
        {
            return this._telegramApi.SendMessage(
                token: this._token,
                chatId: this.ChatId,
                message: message
            );
        }
    }
}
