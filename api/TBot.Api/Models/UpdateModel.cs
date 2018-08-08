using Newtonsoft.Json;

namespace TBot.Api.Models
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

    public class UserModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("language_code")]
        public string LanguageCode { get; set; }
    }

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

    public class ChatModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
