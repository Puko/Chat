namespace Chat.App.JWT;

public sealed record JwtToken(string AccessToken, DateTime ExpiresAtUtc);
