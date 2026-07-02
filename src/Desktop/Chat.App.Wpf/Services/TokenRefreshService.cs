using Chat.App.Contract.Auth;
using Chat.App.Wpf.Models;
using ChatApp.Http;

namespace Chat.App.Wpf.Services;

public sealed class TokenRefreshService(
    JsonHttpClient httpClient,
    TokenStorageService tokenStorage,
    AppSession session)
{
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<bool> TryRefreshAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var refreshToken = await tokenStorage.GetRefreshTokenAsync();
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            var response = await httpClient.PostAsync<RefreshTokenRequestDto, LoginResponseDto>(
                "api/auth/refresh",
                new RefreshTokenRequestDto { RefreshToken = refreshToken },
                ct);

            if (response is null || string.IsNullOrWhiteSpace(response.AccessToken))
                return false;

            await tokenStorage.SaveTokensAsync(response.AccessToken, response.RefreshToken);
            session.SetUser(response.User.Id, response.User.Username);
            return true;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _lock.Release();
        }
    }
}
