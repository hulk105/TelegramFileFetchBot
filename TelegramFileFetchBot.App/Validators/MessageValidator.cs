using FluentValidation;
using Telegram.Bot.Types;
using TelegramFileFetchBot.App.Config;

namespace TelegramFileFetchBot.App.Validators;

/// <summary>
/// Represents a class for validating Telegram messages.
/// </summary>
public class MessageValidator : AbstractValidator<Message?>
{
    public MessageValidator(AppConfig config)
    {
        RuleFor(message => message)
            .NotNull()
            .WithMessage("Message is null.")
            .WithSeverity(Severity.Warning)
            .DependentRules(() =>
            {
                RuleFor(message => message!.Chat.Id)
                    .Must(id => config.AllowedChatIds.Contains(id))
                    .WithMessage(message => $"ChatId {message?.Chat.Id} is not allowed. Message: {message?.Text}")
                    .WithSeverity(Severity.Warning)
                    ;

                RuleFor(message => message!.From)
                    .NotNull()
                    .DependentRules(() =>
                    {
                        RuleFor(message => message!.From!.Id)
                            .Must(from => config.AllowedFromIds.Contains(from))
                            .WithMessage(message => $"From {message?.From?.Id} is not allowed.")
                            .WithSeverity(Severity.Warning)
                            ;
                    })
                    ;
            })
            ;
    }
}