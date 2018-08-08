using System.Threading.Tasks;

namespace TBot.Infrastructure.Bots.Handlers.Context
{
    public interface IBotHandlerContext
    {
        int MessageId { get; }
        int ChatId { get; }
        Task SendMessage(string message);
    }
}