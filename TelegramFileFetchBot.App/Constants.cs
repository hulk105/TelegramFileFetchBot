namespace TelegramFileFetchBot.App;

public static class Constants
{
    public const string SerilogOutputTemplate = "{Timestamp:yyyy-dd-MM HH:mm:ss.fff} [{Level:u3}] <{SourceContext}> {Message}{NewLine}{Exception}";
}