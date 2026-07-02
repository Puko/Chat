using Chat.App.Application.Chat.Commands;
using FluentValidation;

namespace Chat.App.Application.Chat.Validation;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(command => command.Content)
            .NotEmpty()
            .MaximumLength(4000);
    }
}