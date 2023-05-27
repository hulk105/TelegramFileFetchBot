using FluentValidation;
using Telegram.Bot.Types;
using TelegramFileFetchBot.App.Config;

namespace TelegramFileFetchBot.App.Validators;

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
                    .WithMessage(id => $"ChatId {id.Chat.Id} is not allowed.")
                    .WithSeverity(Severity.Warning)
                    ;

                RuleFor(message => message!.From)
                    .NotNull()
                    .DependentRules(() =>
                    {
                        RuleFor(message => message!.From!.Id)
                            .Must(from => config.AllowedFromIds.Contains(from))
                            .WithMessage(id => $"From {id.From.Id} is not allowed.")
                            .WithSeverity(Severity.Warning)
                            ;
                    })
                    ;
            })
            ;
    }
}