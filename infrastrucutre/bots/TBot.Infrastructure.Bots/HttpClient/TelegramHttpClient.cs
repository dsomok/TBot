using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Infrastructure.Bots.HttpClient
{
    class TelegramHttpClient : ITelegramHttpClient
    {
        private readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();
        private readonly ISerializer _serializer;


        public TelegramHttpClient(ISerializer serializer)
        {
            _serializer = serializer;
        }


        public async Task<dynamic> Post(Uri uri, object body = null)
        {
            var json = this._serializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await this._httpClient.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to POST request to Telegram API");
            }

            var responseString = await response.Content.ReadAsStringAsync();

            var responseData = this._serializer.Deserialize<dynamic>(responseString);
            if (responseData.ok != true)
            {
                throw new Exception("Telegram API request was not successfull");
            }

            return responseData.result;
        }
    }
}
