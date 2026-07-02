using Chat.App.Database.Abstracts;
using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstraction.Abstracts;
using Chat.App.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.App.Database.Repositories;

public sealed class RefreshTokenRepository(IDbContextAccessor dbContextAccessor)
    : GenericRepository<RefreshToken>(dbContextAccessor), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetActiveByHashAsync(string tokenHash, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        return await Accessor.Set<RefreshToken>()
            .FirstOrDefaultAsync(
                token => token.TokenHash == tokenHash
                    && token.RevokedAtUtc == null
                    && token.ExpiresAtUtc > now,
                ct);
    }
}
