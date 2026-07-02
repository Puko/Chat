using Chat.App.Application.Users.Commands;
using FluentValidation;

namespace Chat.App.Application.Users.Validation;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.User.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9._-]+$");

        RuleFor(x => x.User.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);
    }
}
