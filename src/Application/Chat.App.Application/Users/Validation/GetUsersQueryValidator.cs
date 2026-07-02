using Chat.App.Application.Users.Queries;
using FluentValidation;

namespace Chat.App.Application.Users.Validation;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.Page.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.Page.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Search)
            .MaximumLength(50)
            .When(x => x.Search is not null);
    }
}
