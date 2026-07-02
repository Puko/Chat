using Chat.App.Application.Chat.Commands;
using FluentValidation;

namespace Chat.App.Application.Chat.Validation;

public sealed class GetOrCreateDirectConversationCommandValidator : AbstractValidator<GetOrCreateDirectConversationCommand>
{
    public GetOrCreateDirectConversationCommandValidator()
    {
        RuleFor(command => command.OtherUserId)
            .NotEmpty();
    }
}
