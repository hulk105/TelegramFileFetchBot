using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFileFetchBot.App.Services;

namespace TelegramFileFetchBot.App.Mediatr.Requests;

/// <summary>
/// Represents a request to download media with a reply in a chat.
/// </summary>
public record DownloadMediaWithReplyRequest(
    long ChatId,
    int ReplyToMessageId,
    string? FileName,
    FileBase FileBase
) : IRequest<Unit>;

public class DownloadMediaWithReplyRequestHandler : IRequestHandler<DownloadMediaWithReplyRequest, Unit>
{
    private readonly ILogger<DownloadMediaWithReplyRequestHandler> _logger;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IFileDownloadService _fileDownloadService;

    public DownloadMediaWithReplyRequestHandler(
        ILogger<DownloadMediaWithReplyRequestHandler> logger,
        ITelegramBotClient telegramBotClient,
        IFileDownloadService fileDownloadService
    )
    {
        _logger = logger;
        _telegramBotClient = telegramBotClient;
        _fileDownloadService = fileDownloadService;
    }

    public async Task<Unit> Handle(DownloadMediaWithReplyRequest request, CancellationToken cancellationToken)
    {
        var reply = await _telegramBotClient.SendTextMessageAsync(
            request.ChatId,
            Resources.Messages.DownloadingMedia,
            replyToMessageId: request.ReplyToMessageId,
            cancellationToken: cancellationToken,
            allowSendingWithoutReply: true
        );

        _logger.LogInformation("Downloading {FileId} from chat {ChatId}", request.FileBase.FileUniqueId, request.ChatId);

        try
        {
            await _fileDownloadService.DownloadFileAsync(request.FileBase, request.FileName, cancellationToken);
            await _telegramBotClient.EditMessageTextAsync(request.ChatId, reply.MessageId, Resources.Messages.DownloadSuccess,
                cancellationToken: cancellationToken);
        }
        catch (Exception)
        {
            await _telegramBotClient.EditMessageTextAsync(request.ChatId, reply.MessageId, Resources.Messages.DownloadFailed,
                cancellationToken: cancellationToken);
            throw;
        }

        return Unit.Value;
    }
}