using System;

namespace TBot.Infrastructure.Bots.Contracts.Exceptions
{
    public class BotCommandHandlerNotFound : Exception
    {
        private readonly string _command;


        public BotCommandHandlerNotFound(string command)
        {
            _command = command;
        }


        public override string Message => $"Handler for command \"{this._command}\" was not found";
    }
}