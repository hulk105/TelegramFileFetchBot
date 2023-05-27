using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace TelegramFileFetchBot.App.Mediatr.Pipelines;

public class ExceptionLoggingHandler<TRequest, TResponse> : IRequestExceptionHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExceptionLoggingHandler<TRequest, TResponse>> _logger;

    public ExceptionLoggingHandler(ILogger<ExceptionLoggingHandler<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, Exception exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, exception.Message);

        state.SetHandled(default!);

        return Task.CompletedTask;
    }
}