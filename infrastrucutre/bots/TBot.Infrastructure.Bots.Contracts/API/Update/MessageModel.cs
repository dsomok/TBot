using Newtonsoft.Json;

namespace TBot.Infrastructure.Bots.Contracts.API.Update
{
    public class MessageModel
    {
        [JsonProperty("message_id")]
        public int MessageId { get; set; }

        [JsonProperty("from")]
        public UserModel From { get; set; }

        [JsonProperty("date")]
        public int Date { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("chat")]
        public ChatModel Chat { get; set; }
    }
}