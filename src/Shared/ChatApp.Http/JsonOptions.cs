using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatApp.Http;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions Value = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public static JsonSerializerOptions Create(params JsonConverter[] converters)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        foreach (var converter in converters)
        {
            options.Converters.Add(converter);
        }

        return options;
    }
}
