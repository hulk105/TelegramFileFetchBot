using FluentValidation;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramFileFetchBot.App.Mediatr.Requests;

/// <summary>
/// Represents a request to handle a message received from Telegram.
/// </summary>
public record MessageRequest(Message Message) : IRequest<Unit>;

public class MessageRequestHandler : IRequestHandler<MessageRequest, Unit>
{
    private readonly IMediator _mediator;

    public MessageRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(MessageRequest request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        switch (message.Type)
        {
            case MessageType.Photo:
                await _mediator.Send(new DownloadPhotoRequest(request.Message), cancellationToken);
                break;
            case MessageType.Video:
                await _mediator.Send(new DownloadVideoRequest(request.Message), cancellationToken);
                break;
        }

        return Unit.Value;
    }
}

public class MessageRequestValidator : AbstractValidator<MessageRequest>
{
    public MessageRequestValidator(IValidator<Message?> messageValidator)
    {
        RuleFor(request => request.Message)
            .SetValidator(messageValidator)
            ;
    }
}