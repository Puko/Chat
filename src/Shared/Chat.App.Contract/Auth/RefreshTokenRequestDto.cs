namespace Chat.App.Contract.Auth;

public sealed class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = null!;
}
