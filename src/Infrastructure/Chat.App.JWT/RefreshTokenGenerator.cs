using System.Security.Cryptography;
using System.Text;

namespace Chat.App.JWT;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private const int TokenSizeInBytes = 64;

    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(TokenSizeInBytes);

        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
