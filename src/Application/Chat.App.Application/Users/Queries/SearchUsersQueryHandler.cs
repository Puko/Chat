using Chat.App.Application.Common;
using Chat.App.Application.Paging;
using Chat.App.Application.Users.Models;
using Chat.App.Database.Abstracts;
using MediatR;

namespace Chat.App.Application.Users.Queries;

public sealed class SearchUsersQueryHandler(
    ICurrentUser currentUser,
    IUserRepository userRepository)
    : IRequestHandler<SearchUsersQuery, PagedResult<UserLookup>>
{
    public async Task<PagedResult<UserLookup>> Handle(SearchUsersQuery request, CancellationToken ct)
    {
        var userId = currentUser.RequireUserId();

        var (entities, totalCount) = await userRepository.SearchAsync(
            request.Search,
            request.Page.PageNumber,
            request.Page.PageSize,
            excludeUserId: userId,
            ct);

        var items = entities
            .Select(entity => new UserLookup(entity.Id, entity.Username))
            .ToList();

        return new PagedResult<UserLookup>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.Page.PageNumber,
            PageSize = request.Page.PageSize
        };
    }
}
