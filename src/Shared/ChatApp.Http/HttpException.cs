using System.Net;
using System.Text.Json;

namespace ChatApp.Http;

public class HttpException(string message, HttpStatusCode statusCode)
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public string Info =>
        $"{(string.IsNullOrEmpty(Message) ? "Please check logs for more details." : Message)} Status Code: {StatusCode}";

    public T? To<T>()
    {
        try
        {
            return JsonSerializer.Deserialize<T>(Message, JsonOptions.Value);
        }
        catch
        {
            // ignored
        }

        return default;
    }
}
