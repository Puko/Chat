using Chat.App.Database.Abstraction;
using Chat.App.Database.Entities;

namespace Chat.App.Database.Abstracts;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetActiveByHashAsync(string tokenHash, CancellationToken ct = default);
}
