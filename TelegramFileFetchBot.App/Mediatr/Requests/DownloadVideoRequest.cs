using FluentValidation;
using MediatR;
using Telegram.Bot.Types;

namespace TelegramFileFetchBot.App.Mediatr.Requests;

/// <summary>
/// Represents a request to download a video from Telegram.
/// </summary>
public record DownloadVideoRequest(Message Message) : IRequest<Unit>;

public class DownloadVideoRequestHandler : IRequestHandler<DownloadVideoRequest, Unit>
{
    private readonly IMediator _mediator;

    public DownloadVideoRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(DownloadVideoRequest request, CancellationToken cancellationToken)
    {
        var video = request.Message.Video;
        var downloadRequest = new DownloadMediaWithReplyRequest(request.Message.Chat.Id, request.Message.MessageId,
            request.Message.Caption, video!);

        await _mediator.Send(downloadRequest, cancellationToken);

        return Unit.Value;
    }
}

public class DownloadVideoRequestValidator : AbstractValidator<DownloadVideoRequest>
{
    public DownloadVideoRequestValidator()
    {
        RuleFor(request => request.Message.Video)
            .NotNull();
    }
}