using Newtonsoft.Json;

namespace TBot.Infrastructure.Bots.Contracts.API.Update
{
    public class ChatModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}