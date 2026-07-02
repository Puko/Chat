using Chat.App.Contract.Users;

namespace Chat.App.Contract.Auth;

public sealed record LoginResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string RefreshToken,
    UserDto User);
