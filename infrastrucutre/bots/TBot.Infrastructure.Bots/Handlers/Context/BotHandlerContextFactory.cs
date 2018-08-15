using TBot.Infrastructure.Bots.API;
using TBot.Infrastructure.Bots.Contracts.Messages;

namespace TBot.Infrastructure.Bots.Handlers.Context
{
    class BotHandlerContextFactory : IBotHandlerContextFactory
    {
        private readonly string _token;
        private readonly ITelegramApi _telegramApi;


        public BotHandlerContextFactory(string token, ITelegramApi telegramApi)
        {
            _token = token;
            _telegramApi = telegramApi;
        }


        public IBotHandlerContext Create<TCommand>(TCommand command) where TCommand : class, IBotCommand
        {
            return new BotHandlerContext(
                chatId: command.ChatId,
                messageId: command.MessageId,
                token: this._token,
                telegramApi:this._telegramApi
            );
        }
    }
}