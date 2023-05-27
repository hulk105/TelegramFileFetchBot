namespace TelegramFileFetchBot.App.Config;

public class AppConfig
{
    public string BotToken { get; set; } = string.Empty;
    public long?[] AllowedFromIds { get; set; } = Array.Empty<long?>();
    public long[] AllowedChatIds { get; set; } = Array.Empty<long>();
}