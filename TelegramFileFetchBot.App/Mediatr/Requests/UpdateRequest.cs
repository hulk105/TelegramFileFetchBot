﻿using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramFileFetchBot.App.Mediatr.Requests;

/// <summary>
/// Represents a request to handle an update received from Telegram.
/// </summary>
public record UpdateRequest(Update Update) : IRequest<Unit>;

public class UpdateRequestHandler : IRequestHandler<UpdateRequest, Unit>
{
    private readonly IMediator _mediator;

    public UpdateRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(UpdateRequest request, CancellationToken cancellationToken)
    {
        var update = request.Update;

        switch (update.Type)
        {
            case UpdateType.Message:
                await _mediator.Send(new MessageRequest(request.Update.Message!), cancellationToken);
                break;
        }

        return Unit.Value;
    }
}