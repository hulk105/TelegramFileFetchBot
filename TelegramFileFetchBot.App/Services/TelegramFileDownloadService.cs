using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace TelegramFileFetchBot.App.Services;

public interface IFileDownloadService
{
    /// <summary>
    /// Downloads a file asynchronously.
    /// </summary>
    /// <param name="fileBase">The file to download.</param>
    /// <param name="fileName">The name to save the file with.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DownloadFileAsync(FileBase fileBase, string? fileName, CancellationToken cancellationToken);
}

/// <summary>
/// Represents a service for downloading files from Telegram.
/// </summary>
public partial class TelegramFileDownloadService : IFileDownloadService
{
    private readonly ILogger<TelegramFileDownloadService> _logger;
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramFileDownloadService(ILogger<TelegramFileDownloadService> logger, ITelegramBotClient telegramBotClient)
    {
        _logger = logger;
        _telegramBotClient = telegramBotClient;
    }

    /// <inheritdoc />
    public async Task DownloadFileAsync(
        FileBase fileBase,
        string? fileName,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var file = await _telegramBotClient.GetFileAsync(fileBase.FileUniqueId, cancellationToken);

            if (file.FilePath is not null)
            {
                fileName = GetFormattedFileName(file.FileUniqueId, Path.GetFileName(file.FilePath));

                await using var stream = File.OpenWrite(fileName);

                    _logger.LogInformation("Downloading file: {@File} as {FileName}", fileBase, fileName);
                    await _telegramBotClient.DownloadFileAsync(file.FilePath, stream, cancellationToken);
                    _logger.LogInformation("File downloaded successfully: {@FileUniqueId}", fileBase.FileUniqueId);
            }
        }
        catch (Exception)
        {
            _logger.LogInformation("Failed to download file: {@File}", fileBase);
            throw;
        }
    }

    /// <summary>
    /// Gets the formatted file name by appending the file ID and removing escape symbols from the original file name.
    /// </summary>
    /// <param name="fileId">The unique identifier of the file.</param>
    /// <param name="fileName">The original file name.</param>
    /// <returns>The formatted file name.</returns>
    private static string GetFormattedFileName(string fileId, string fileName)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendJoin('_', fileId, EscapeSymbols().Replace(fileName, string.Empty));
        return stringBuilder.ToString();
    }

    [GeneratedRegex("\\t|\\n|\\r")]
    private static partial Regex EscapeSymbols();
}