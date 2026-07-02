namespace Chat.App.Application.Users;

public sealed record IssuedRefreshToken(string Token, DateTime ExpiresAtUtc);

public interface IRefreshTokenIssuer
{
    Task<IssuedRefreshToken> IssueAsync(Guid userId, CancellationToken ct = default);
}
