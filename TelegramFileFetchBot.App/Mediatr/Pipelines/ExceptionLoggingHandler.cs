using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace TelegramFileFetchBot.App.Mediatr.Pipelines;

/// <summary>
/// A handler for logging exceptions that occur during MediatR request processing.
/// </summary>
/// <typeparam name="TRequest">The type of the request being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
/// <typeparam name="TException">The type of the exception being handled.</typeparam>
public class ExceptionLoggingHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<TResponse>
    where TException : Exception
{
    private readonly ILogger<ExceptionLoggingHandler<TRequest, TResponse, TException>> _logger;

    public ExceptionLoggingHandler(ILogger<ExceptionLoggingHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        TRequest request,
        TException exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken
    )
    {
        switch (exception)
        {
            case ValidationException validationException:
                HandleValidationException(validationException);
                break;
            default:
                HandleException(exception);
                break;
        }

        state.SetHandled(default!);

        return Task.CompletedTask;
    }

    private void HandleException(Exception exception)
    {
        _logger.LogError(exception, exception.Message);
    }

    private void HandleValidationException(ValidationException exception)
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
    }
}