using FluentValidation;
using MediatR;
using Telegram.Bot.Types;

namespace TelegramFileFetchBot.App.Mediatr.Requests;

public record PhotoRequest(Update Update) : IRequest<Unit>;

public class PhotoRequestHandler : IRequestHandler<PhotoRequest, Unit>
{
    private readonly IMediator _mediator;

    public PhotoRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(PhotoRequest request, CancellationToken cancellationToken)
    {
        var maxPhotoSize = request.Update.Message!.Photo!.MaxBy(x => x.Width * x.Height);

        await _mediator.Send(new DownloadMediaWithReplyRequest(request.Update.Message, maxPhotoSize!), cancellationToken);
        
        return Unit.Value;
    }
}

public class PhotoRequestValidator : AbstractValidator<PhotoRequest>
{
    public PhotoRequestValidator(IValidator<Message?> messageValidator, IValidator<PhotoSize[]?> photoSizeValidator)
    {
        RuleFor(update => update.Update.Message)
            .SetValidator(messageValidator)
            .DependentRules(() =>
            {
                RuleFor(update => update.Update.Message!.Photo)
                    .SetValidator(photoSizeValidator);
            })
            ;
    }
}