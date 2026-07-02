using Chat.App.Database.Abstraction;
using Chat.App.Database.Entities;

namespace Chat.App.Database.Abstracts;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);

    Task<(IReadOnlyList<User> Items, int TotalCount)> SearchAsync(
        string? search,
        int pageNumber,
        int pageSize,
        Guid? excludeUserId = null,
        CancellationToken ct = default);
}
