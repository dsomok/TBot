using System;
using System.Collections.Generic;
using System.Text;

namespace TBot.Infrastructure.Bots.Contracts.Exceptions
{
    public class UnsupportedCommandException : Exception
    {
        private readonly string _command;


        public UnsupportedCommandException(string command)
        {
            _command = command;
        }


        public override string Message => $"Command \"{this._command}\" is not supported";
    }
}
