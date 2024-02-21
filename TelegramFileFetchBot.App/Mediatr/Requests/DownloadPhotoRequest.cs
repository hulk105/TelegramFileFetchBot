using FluentValidation;
using MediatR;
using Telegram.Bot.Types;

namespace TelegramFileFetchBot.App.Mediatr.Requests;

/// <summary>
/// Represents a request to download a photo from Telegram.
/// </summary>
/// <param name="Message">The message containing the photo to be downloaded.</param>
public record DownloadPhotoRequest(Message Message) : IRequest<Unit>;

public class DownloadPhotoRequestHandler : IRequestHandler<DownloadPhotoRequest, Unit>
{
    private readonly IMediator _mediator;

    public DownloadPhotoRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(DownloadPhotoRequest request, CancellationToken cancellationToken)
    {
        var photo = request.Message.Photo!.MaxBy(x => x.Width * x.Height);
        var downloadRequest = new DownloadMediaWithReplyRequest(request.Message.Chat.Id, request.Message.MessageId, 
            request.Message.Caption, photo!);

        await _mediator.Send(downloadRequest, cancellationToken);

        return Unit.Value;
    }
}

public class DownloadPhotoRequestValidator : AbstractValidator<DownloadPhotoRequest>
{
    public DownloadPhotoRequestValidator(IValidator<PhotoSize[]?> photoSizeValidator)
    {
        RuleFor(request => request.Message.Photo)
            .SetValidator(photoSizeValidator);
    }
}

public class PhotoSizeValidator : AbstractValidator<PhotoSize[]?>
{
    public PhotoSizeValidator()
    {
        RuleFor(photos => photos)
            .NotEmpty()
            ;
    }
}