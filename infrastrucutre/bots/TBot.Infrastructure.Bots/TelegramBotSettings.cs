namespace TBot.Infrastructure.Bots
{
    public class TelegramBotSettings
    {
        public TelegramBotSettings(string name, string token)
        {
            Name = name;
            Token = token;
        }

        public string Name { get; }
        public string Token { get; }
    }
}
