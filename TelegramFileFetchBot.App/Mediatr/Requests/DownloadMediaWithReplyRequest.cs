using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFileFetchBot.App.Services;

namespace TelegramFileFetchBot.App.Mediatr.Requests;

public record DownloadMediaWithReplyRequest(
    Message? Message,
    FileBase FileBase
) : IRequest<Unit>;

public class DownloadMediaWithReplyRequestHandler : IRequestHandler<DownloadMediaWithReplyRequest, Unit>
{
    private readonly ILogger<DownloadMediaWithReplyRequestHandler> _logger;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IFileDownloadService _fileDownloadService;

    public DownloadMediaWithReplyRequestHandler(ILogger<DownloadMediaWithReplyRequestHandler> logger, ITelegramBotClient telegramBotClient, IFileDownloadService fileDownloadService)
    {
        _logger = logger;
        _telegramBotClient = telegramBotClient;
        _fileDownloadService = fileDownloadService;
    }

    public async Task<Unit> Handle(DownloadMediaWithReplyRequest request, CancellationToken cancellationToken)
    {
        var reply = await _telegramBotClient.SendTextMessageAsync(
            request.Message!.Chat.Id,
            Resources.Messages.DownloadingMedia,
            replyToMessageId: request.Message.MessageId,
            cancellationToken: cancellationToken,
            allowSendingWithoutReply: true
        );

        _logger.LogInformation("Downloading {MessageId} in chat {ChatId} from {FromId}",
            request.Message.MessageId, request.Message.Chat.Id, request.Message.From!.Username);

        try
        {
            await _fileDownloadService.DownloadFileAsync(request.FileBase.FileId, request.Message.Caption, cancellationToken);
            await _telegramBotClient
                .EditMessageTextAsync(request.Message.Chat.Id, reply.MessageId, Resources.Messages.DownloadSuccess, cancellationToken: cancellationToken);
        }
        catch (Exception)
        {
            await _telegramBotClient
                .EditMessageTextAsync(request.Message.Chat.Id, reply.MessageId, Resources.Messages.DownloadFailed, cancellationToken: cancellationToken);
        }
        
        return Unit.Value;
    }
}

public class DownloadMediaWithReplyRequestValidator : AbstractValidator<DownloadMediaWithReplyRequest>
{
    public DownloadMediaWithReplyRequestValidator(IValidator<Message?> messageValidator)
    {
        RuleFor(request => request.Message)
            .SetValidator(messageValidator)
            ;
    }
}