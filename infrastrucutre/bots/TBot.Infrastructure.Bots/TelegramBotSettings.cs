namespace TBot.Infrastructure.Bots
{
    public class TelegramBotSettings
    {
        public TelegramBotSettings(string name, string token, string apiURL)
        {
            Name = name;
            Token = token;
            ApiURL = apiURL;
        }

        public string Name { get; }
        public string Token { get; }
        public string ApiURL { get; }
    }
}
