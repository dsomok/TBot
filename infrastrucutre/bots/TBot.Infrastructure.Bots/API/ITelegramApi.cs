using System.Collections.Generic;
using System.Threading.Tasks;

namespace TBot.Infrastructure.Bots.Api
{
    public interface ITelegramApi
    {
        Task<bool> SetWebhook(string token, string webHookUrl, int maxConnections = 40);
        Task<bool> SetWebhook(string token, string webHookUrl, int maxConnections, IEnumerable<string> allowedUpdates);
        Task<bool> DeleteWebhook(string token);
    }
}