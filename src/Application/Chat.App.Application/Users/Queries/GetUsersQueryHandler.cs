using Chat.App.Application.Common;
using Chat.App.Application.Paging;
using Chat.App.Application.Users.Models;
using Chat.App.Database.Abstracts;
using MediatR;
using DbUserRole = Chat.App.Database.Entities.UserRole;

namespace Chat.App.Application.Users.Queries;

public sealed class GetUsersQueryHandler(IUserRepository userRepository, ICurrentUser currentUser)
    : IRequestHandler<GetUsersQuery, PagedResult<User>>
{
    public async Task<PagedResult<User>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        currentUser.RequireAdmin();

        var (entities, totalCount) = await userRepository.SearchAsync(
            request.Search,
            request.Page.PageNumber,
            request.Page.PageSize,
            excludeUserId: null,
            ct);

        var items = entities
            .Select(entity => new User
            {
                Id = entity.Id,
                Username = entity.Username,
                Role = MapRole(entity.Role),
                CreatedAtUtc = entity.CreatedAtUtc
            })
            .ToList();

        return new PagedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.Page.PageNumber,
            PageSize = request.Page.PageSize
        };
    }

    private static UserRole MapRole(DbUserRole role) =>
        role switch
        {
            DbUserRole.Admin => UserRole.Admin,
            _ => UserRole.User
        };
}
