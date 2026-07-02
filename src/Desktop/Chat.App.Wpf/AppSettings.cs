using System.IO;
using System.Text.Json;

namespace Chat.App.Wpf;

internal static class AppSettings
{
    public static string ApiBaseUrl { get; } = LoadApiBaseUrl();

    private static string LoadApiBaseUrl()
    {
        const string defaultUrl = "http://localhost:5242";
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        if (!File.Exists(path))
            return defaultUrl;

        try
        {
            using var stream = File.OpenRead(path);
            using var document = JsonDocument.Parse(stream);

            if (document.RootElement.TryGetProperty("ApiBaseUrl", out var value)
                && value.GetString() is { Length: > 0 } url)
            {
                return url;
            }
        }
        catch
        {
        }

        return defaultUrl;
    }
}
