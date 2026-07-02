using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstracts;
using Chat.App.JWT;
using Microsoft.Extensions.Options;
using DbRefreshToken = Chat.App.Database.Entities.RefreshToken;

namespace Chat.App.Application.Users;

public sealed class RefreshTokenIssuer(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IRefreshTokenGenerator refreshTokenGenerator,
    IOptions<JwtOptions> jwtOptions) : IRefreshTokenIssuer
{
    public async Task<IssuedRefreshToken> IssueAsync(Guid userId, CancellationToken ct = default)
    {
        var rawToken = refreshTokenGenerator.Generate();
        var expiresAtUtc = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationDays);

        await refreshTokenRepository.AddAsync(new DbRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = refreshTokenGenerator.Hash(rawToken),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = expiresAtUtc
        }, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return new IssuedRefreshToken(rawToken, expiresAtUtc);
    }
}
