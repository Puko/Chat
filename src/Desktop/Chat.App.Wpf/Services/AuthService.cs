using System.IdentityModel.Tokens.Jwt;
using Chat.App.Contract.Auth;
using Chat.App.Wpf.Models;
using Chat.App.Wpf.Services.Http;
using Chat.App.Wpf.Services.Http.ErrorMessages;
using ChatApp.Http;

namespace Chat.App.Wpf.Services;

public sealed class AuthService(
    JsonHttpClient httpClient,
    TokenStorageService tokenStorage,
    HttpErrorHandler errorHandler,
    TokenRefreshService tokenRefresh,
    AppSession session)
    : BaseApiService(httpClient, tokenStorage, errorHandler)
{
    protected override IReadOnlyList<ErrorCodeMessageResolver> ErrorResolvers { get; } =
    [
        AuthErrorMessages.TryResolve,
        UserErrorMessages.TryResolve
    ];

    public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto request,
        CancellationToken ct = default)
    {
        var result = await ExecuteAsync(async () =>
        {
            var auth =
                await Http.PostAsync<LoginRequestDto, LoginResponseDto>("api/auth/login", request,
                    ct);

            if (auth is null || string.IsNullOrWhiteSpace(auth.AccessToken))
                throw new InvalidOperationException("Login failed.");

            await TokenStorage.SaveTokensAsync(auth.AccessToken, auth.RefreshToken);
            session.SetUser(auth.User.Id, auth.User.Username);
            return auth;
        }, ct);

        return result;
    }

    public async Task<ServiceResult<LoginResponseDto>> RegisterAsync(RegisterUser request,
        CancellationToken ct = default)
    {
        var registerResult = await ExecuteAsync(async () =>
        {
            var response =
                await Http.PostAsync<RegisterUser, RegisterUserResponseDto>("api/auth/register",
                    request, ct);

            if (response is null)
                throw new InvalidOperationException("Registration failed.");

            return response;
        }, ct);

        if (!registerResult.IsSuccess)
            return ServiceResult<LoginResponseDto>.Fail(registerResult.Error!);

        return await LoginAsync(new LoginRequestDto
        {
            Username = request.Username,
            Password = request.Password
        }, ct);
    }

    public async Task LogoutAsync()
    {
        await TokenStorage.RemoveTokensAsync();
        session.Clear();
    }

    public async Task<bool> TryRestoreSessionAsync(CancellationToken ct = default)
    {
        var accessToken = await TokenStorage.GetAccessTokenAsync();

        if (!string.IsNullOrWhiteSpace(accessToken) && TryReadClaims(accessToken, out var userId, out var username))
        {
            session.SetUser(userId, username);
            return true;
        }

        return await tokenRefresh.TryRefreshAsync(ct);
    }

    private static bool TryReadClaims(string token, out Guid userId, out string username)
    {
        userId = Guid.Empty;
        username = "";

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            if (jwt.ValidTo <= DateTime.UtcNow)
                return false;

            var subject = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var name = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

            if (!Guid.TryParse(subject, out userId) || string.IsNullOrWhiteSpace(name))
                return false;

            username = name;
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
