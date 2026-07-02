using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Chat.App.Wpf.Services;

public sealed class TokenStorageService
{
    private sealed record StoredTokens(string AccessToken, string RefreshToken);

    private static readonly string StorageDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Chat.App.Wpf");

    private static readonly string StorageFilePath = Path.Combine(StorageDirectory, "auth.token");

    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("Chat.App.Wpf.TokenStorage");

    private StoredTokens? _cached;

    public async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        _cached = new StoredTokens(accessToken, refreshToken);

        Directory.CreateDirectory(StorageDirectory);
        var json = JsonSerializer.Serialize(_cached);
        var protectedBytes = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(json), Entropy, DataProtectionScope.CurrentUser);

        await File.WriteAllBytesAsync(StorageFilePath, protectedBytes);
    }

    public async Task<string?> GetAccessTokenAsync()
        => (await LoadAsync())?.AccessToken;

    public async Task<string?> GetRefreshTokenAsync()
        => (await LoadAsync())?.RefreshToken;

    public Task RemoveTokensAsync()
    {
        _cached = null;

        if (File.Exists(StorageFilePath))
            File.Delete(StorageFilePath);

        return Task.CompletedTask;
    }

    private async Task<StoredTokens?> LoadAsync()
    {
        if (_cached is not null)
            return _cached;

        if (!File.Exists(StorageFilePath))
            return null;

        try
        {
            var protectedBytes = await File.ReadAllBytesAsync(StorageFilePath);
            var bytes = ProtectedData.Unprotect(protectedBytes, Entropy, DataProtectionScope.CurrentUser);
            var json = Encoding.UTF8.GetString(bytes);

            _cached = JsonSerializer.Deserialize<StoredTokens>(json);
            return _cached;
        }
        catch (Exception ex) when (ex is CryptographicException or JsonException)
        {
            await RemoveTokensAsync();
            return null;
        }
    }
}
