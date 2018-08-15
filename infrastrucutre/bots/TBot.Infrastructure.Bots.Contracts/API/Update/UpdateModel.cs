using Newtonsoft.Json;

namespace TBot.Infrastructure.Bots.Contracts.API.Update
{
    public class UpdateModel
    {
        [JsonProperty("update_id")]
        public int UpdateId { get; set; }

        [JsonProperty("message")]
        public MessageModel Message { get; set; }

        [JsonProperty("edited_message")]
        public MessageModel EditedMessage { get; set; }

        [JsonProperty("channel_post")]
        public MessageModel ChannelPost { get; set; }

        [JsonProperty("edited_channel_post")]
        public MessageModel EditedChannelPost { get; set; }
    }
}
