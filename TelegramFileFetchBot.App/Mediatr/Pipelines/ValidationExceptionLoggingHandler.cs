using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace TelegramFileFetchBot.App.Mediatr.Pipelines;

public class ValidationExceptionLoggingHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, ValidationException>
    where TRequest : IRequest<TResponse>
    where TException : ValidationException
{
    private readonly ILogger<ValidationExceptionLoggingHandler<TRequest, TResponse, TException>> _logger;

    public ValidationExceptionLoggingHandler(ILogger<ValidationExceptionLoggingHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        TRequest request,
        ValidationException exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        foreach (var error in exception.Errors)
        {
            switch (error.Severity)
            {
                case Severity.Info:
                    _logger.LogInformation(error.ErrorMessage);
                    break;
                case Severity.Warning:
                    _logger.LogWarning(error.ErrorMessage);
                    break;
                case Severity.Error:
                    _logger.LogError(error.ErrorMessage);
                    break;
            }
        }

        state.SetHandled(default!);

        return Task.CompletedTask;
    }
}