using System;
using TBot.Infrastructure.Bots.Commands;
using TBot.Infrastructure.Bots.Contracts.API.Update;
using TBot.Infrastructure.Bots.Contracts.Commands;
using TBot.Infrastructure.Bots.Contracts.Exceptions;
using TBot.Infrastructure.Bots.Contracts.Messages;

namespace TBot.TestBot.Host
{
    internal class TestBotCommandResolver : ICommandResolver
    {
        public IBotCommand CreateCommand(UpdateModel updateModel)
        {
            var command = updateModel.Message.Text;
            if (command == "/start")
            {
                return new StartCommand(updateModel.Message.Chat.Id, updateModel.Message.MessageId);
            }

            throw new UnsupportedCommandException(command);
        }
    }
}
