using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace TelegramFileFetchBot.App.Services;

public interface IFileDownloadService
{
    Task DownloadFileAsync(
        string fileId,
        string? fileName,
        CancellationToken cancellationToken);
}

public class FileDownloadService : IFileDownloadService
{
    private readonly ILogger<FileDownloadService> _logger;
    private readonly ITelegramBotClient _telegramBotClient;

    public FileDownloadService(ILogger<FileDownloadService> logger, ITelegramBotClient telegramBotClient)
    {
        _logger = logger;
        _telegramBotClient = telegramBotClient;
    }

    public async Task DownloadFileAsync(
        string fileId,
        string? fileName,
        CancellationToken cancellationToken)
    {
        var file = await _telegramBotClient.GetFileAsync(fileId, cancellationToken);

        if (file.FilePath != null)
        {
            fileName = GetFormattedFileName(file.FileUniqueId, fileName, Path.GetExtension(file.FilePath));

            await using var stream = File.OpenWrite(fileName);

            try
            {
                _logger.LogInformation("Downloading file: {FileId} as {FileName}", fileId, fileName);
                await _telegramBotClient.DownloadFileAsync(file.FilePath, stream, cancellationToken);
                _logger.LogInformation("File downloaded: {FileId} as {FileName}", fileId, fileName);
            }
            catch (Exception)
            {
                _logger.LogInformation("Failed to download file: {FileId} as {FileName}", fileId, fileName);
                throw;
            }
        }
    }


    private static string GetFormattedFileName(string fileId, string? fileName, string extension)
    {
        var stringBuilder = new StringBuilder(fileId);
        stringBuilder.AppendJoin(string.Empty, "_", Regex.Replace(fileName ?? string.Empty, @"\t|\n|\r", " "));
        stringBuilder.Append(extension);
        return stringBuilder.ToString();
    }
}