namespace Chat.App.Application.Errors;

public static class AuthErrorCodes
{
    public const string InvalidCredentials = "auth.invalid_credentials";
    public const string Unauthenticated = "auth.unauthenticated";
    public const string Forbidden = "auth.forbidden";
    public const string InvalidRefreshToken = "auth.invalid_refresh_token";
}
