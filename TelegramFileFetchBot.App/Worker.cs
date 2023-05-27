using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using TelegramFileFetchBot.App.Mediatr.Requests;

namespace TelegramFileFetchBot.App;

public class Worker
{
    private readonly ILogger<Worker> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly ReceiverOptions _receiverOptions;
    private readonly IServiceProvider _serviceProvider;

    public Worker(
        ILogger<Worker> logger,
        ITelegramBotClient botClient,
        ReceiverOptions receiverOptions,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _botClient = botClient;
        _receiverOptions = receiverOptions;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        var me = await _botClient.GetMeAsync(stoppingToken);
        _logger.LogInformation("Start listening for {Username}", me.Username);
        _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, _receiverOptions, stoppingToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new UpdateRequest(update), cancellationToken);
    }

    private Task HandleErrorAsync(ITelegramBotClient telegramBotClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
            _                                       => exception.Message
        };

        _logger.LogCritical(exception, errorMessage);

        throw exception;
    }
}