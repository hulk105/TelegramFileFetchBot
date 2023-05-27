using FluentValidation;
using Telegram.Bot.Types;

namespace TelegramFileFetchBot.App.Validators;

public class PhotoSizeValidator : AbstractValidator<PhotoSize[]?>
{
    public PhotoSizeValidator()
    {
        RuleFor(photo => photo)
            .NotEmpty()
            ;
    }
}