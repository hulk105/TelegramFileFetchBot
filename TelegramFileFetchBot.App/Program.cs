﻿using System.Reflection;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using TelegramFileFetchBot.App.Config;
using TelegramFileFetchBot.App.Mediatr.Pipelines;
using TelegramFileFetchBot.App.Services;

namespace TelegramFileFetchBot.App;

internal static class Program
{
    private static Assembly Assembly => typeof(Program).Assembly;
    private static readonly CancellationTokenSource TokenSource = new();

    public static async Task Main(string[] args)
    {
        var app = ConfigureApplication(args);

        var worker = app.Services.GetRequiredService<Worker>();

        await worker.StartAsync(TokenSource.Token);

        await app.WaitForShutdownAsync(TokenSource.Token);
    }

    private static IHost ConfigureApplication(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        var config = builder.Configuration.GetSection("TelegramServiceConfig").Get<AppConfig>()!;
        var logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: Constants.SerilogOutputTemplate)
            .CreateLogger();

        builder.Logging
            .ClearProviders()
            .AddSerilog(logger);

        builder.Services
            .AddSingleton(config)
            .AddValidatorsFromAssembly(Assembly)
            .AddMediatR(opts =>
            {
                opts.RegisterServicesFromAssemblies(Assembly);
                opts.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));
            })
            .AddSingleton<ITelegramBotClient>(new TelegramBotClient(config.BotToken))
            .AddSingleton(new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            })
            .AddScoped<IFileDownloadService, FileDownloadService>()
            .AddSingleton<Worker>()
            ;

        var app = builder.Build();
        return app;
    }
}