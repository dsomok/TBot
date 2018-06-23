using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using TBot.Infrastructure.Bots.HttpClient;

namespace TBot.Infrastructure.Bots.Api
{
    internal class TelegramApi : ITelegramApi
    {
        private readonly ITelegramHttpClient _client;
        private readonly ILogger _logger;


        public TelegramApi(ITelegramHttpClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }
        

        public async Task<bool> SetWebhook(string token, string webHookUrl, int maxConnections = 40)
        {
            try
            {
                var uri = this.GetApiUri(token, "setWebhook");
                var body = new
                {
                    url = webHookUrl,
                    max_connections = maxConnections
                };

                await this._client.Post(uri, body);

                return true;
            }
            catch (Exception ex)
            {
                this._logger.Warning(ex, "Failed to set webhook");
                return false;
            }
        }

        public async Task<bool> SetWebhook(string token, string webHookUrl, int maxConnections, IEnumerable<string> allowedUpdates)
        {
            try
            {
                var uri = this.GetApiUri(token, "setWebhook");
                var body = new
                {
                    url = webHookUrl,
                    max_connections = maxConnections,
                    allowed_updates = allowedUpdates.ToArray()
                };

                await this._client.Post(uri, body);

                return true;
            }
            catch (Exception ex)
            {
                this._logger.Warning(ex, "Failed to set webhook");
                return false;
            }
        }

        public async Task<bool> DeleteWebhook(string token)
        {
            try
            {
                var uri = this.GetApiUri(token, "deleteWebhook");

                var result = await this._client.Post(uri);

                return result == true;
            }
            catch (Exception ex)
            {
                this._logger.Warning(ex, "Failed to delete webhook");
                return false;
            }
        }


        private Uri GetApiUri(string token, string command, params KeyValuePair<string, string>[] parameters)
        {
            var uriBuilder = new StringBuilder($"https://api.telegram.org/bot{token}/{command}");
            if (parameters.Length == 0)
            {
                return new Uri(uriBuilder.ToString());
            }

            uriBuilder.Append('?');

            foreach (var parameter in parameters)
            {
                uriBuilder.Append($"{parameter.Key}={parameter.Value}&");
            }

            uriBuilder.Remove(uriBuilder.Length - 1, 1);

            return new Uri(uriBuilder.ToString());
        }
    }
}
