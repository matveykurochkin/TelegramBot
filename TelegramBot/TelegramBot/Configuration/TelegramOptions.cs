namespace TelegramBot.Configuration
{
    internal class TelegramOptions
    {
        public string? Token { get; set; }
        internal bool EsureValid() => !string.IsNullOrEmpty(Token);
    }
}
