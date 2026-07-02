namespace Chat.App.Contract.Auth;

public sealed class LoginRequestDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
