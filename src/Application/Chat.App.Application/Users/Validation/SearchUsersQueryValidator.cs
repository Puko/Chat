using Chat.App.Application.Users.Queries;
using FluentValidation;

namespace Chat.App.Application.Users.Validation;

public sealed class SearchUsersQueryValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersQueryValidator()
    {
        RuleFor(x => x.Page.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.Page.PageSize)
            .InclusiveBetween(1, 50);

        RuleFor(x => x.Search)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);
    }
}
