using Chat.App.Application.Paging;
using Chat.App.Application.Users.Models;
using MediatR;

namespace Chat.App.Application.Users.Queries;

public sealed record GetUsersQuery(Page Page, string? Search) : IRequest<PagedResult<User>>;
public sealed record SearchUsersQuery(Page Page, string Search) : IRequest<PagedResult<UserLookup>>;

