using Chat.App.Database.Abstracts;
using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstraction.Abstracts;
using Chat.App.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.App.Database.Repositories;

public sealed class UserRepository(IDbContextAccessor dbContextAccessor)
    : GenericRepository<User>(dbContextAccessor), IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => await Accessor.Set<User>()
            .FirstOrDefaultAsync(user => user.Username == username, ct);

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> SearchAsync(
        string? search,
        int pageNumber,
        int pageSize,
        Guid? excludeUserId = null,
        CancellationToken ct = default)
    {
        var query = Accessor.Set<User>().AsQueryable();

        if (excludeUserId.HasValue)
            query = query.Where(user => user.Id != excludeUserId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(user => user.Username.Contains(term));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(user => user.Username)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
