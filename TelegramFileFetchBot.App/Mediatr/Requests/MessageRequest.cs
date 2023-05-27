using FluentValidation;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramFileFetchBot.App.Mediatr.Requests;

public record MessageRequest(Update Update) : IRequest<Unit>;

public class MessageRequestValidator : AbstractValidator<MessageRequest>
{
    public MessageRequestValidator(IValidator<Message?> messageValidator)
    {
        RuleFor(request => request.Update.Message)
            .SetValidator(messageValidator)
            ;
    }
}

public class MessageRequestHandler : IRequestHandler<MessageRequest, Unit>
{
    private readonly IMediator _mediator;

    public MessageRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(MessageRequest request, CancellationToken cancellationToken)
    {
        var message = request.Update.Message!;

        switch (message.Type)
        {
            case MessageType.Photo:
                await _mediator.Send(new PhotoRequest(request.Update), cancellationToken);
                break;
            case MessageType.Video:
                break;
        }

        return Unit.Value;
    }
}