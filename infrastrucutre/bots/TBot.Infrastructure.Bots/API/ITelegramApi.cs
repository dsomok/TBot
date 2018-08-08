using System.Collections.Generic;
using System.Threading.Tasks;

namespace TBot.Infrastructure.Bots.API
{
    public interface ITelegramApi
    {
        Task<bool> SetWebhook(string token, string webHookUrl, int maxConnections = 40);
        Task<bool> SetWebhook(string token, string webHookUrl, int maxConnections, IEnumerable<string> allowedUpdates);
        Task<bool> DeleteWebhook(string token);

        Task<bool> SendMessage(string token, int chatId, string message);
    }
}