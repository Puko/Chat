using Chat.App.Application.Users.Commands;
using FluentValidation;

namespace Chat.App.Application.Users.Validation;

public sealed class LoginUserCommandValidator : AbstractValidator<CommandRecords>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Request.Username)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .MaximumLength(100);
    }
}
