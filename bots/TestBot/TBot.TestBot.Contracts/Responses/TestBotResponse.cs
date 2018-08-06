using TBot.Infrastructure.Messaging.Abstractions.Messages;

namespace TBot.TestBot.Contracts.Responses
{
    public class TestBotResponse : IMessage
    {
        public TestBotResponse(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
