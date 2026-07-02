using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ChatApp.Http;

public sealed class JsonHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IResponseMessageInterceptor? _interceptor;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonHttpClient(HttpClient httpClient, JsonSerializerOptions? jsonOptions = null, IResponseMessageInterceptor? interceptor = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = jsonOptions ?? JsonOptions.Value;
        _interceptor = interceptor;

        _httpClient.DefaultRequestHeaders.AcceptCharset.Clear();
        _httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public Task<TResponse?> GetAsync<TResponse>(
        string relativeUrl,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendAsync<TResponse>(HttpMethod.Get, relativeUrl, body: null, cancellationToken, headers);

    public Task<TResponse?> PostAsync<TRequest, TResponse>(
        string relativeUrl,
        TRequest body,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendAsync<TResponse>(HttpMethod.Post, relativeUrl, body, cancellationToken, headers);

    public Task PostAsync<TRequest>(
        string relativeUrl,
        TRequest body,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendAsync<object?>(HttpMethod.Post, relativeUrl, body, cancellationToken, headers);

    public Task<TResponse?> PostAsync<TResponse>(
        string relativeUrl,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendAsync<TResponse?>(HttpMethod.Post, relativeUrl, null, cancellationToken, headers);

    public Task PostAsync(
        string relativeUrl,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendAsync<object?>(HttpMethod.Post, relativeUrl, null, cancellationToken, headers);

    public Task<TResponse?> PutAsync<TRequest, TResponse>(
        string relativeUrl,
        TRequest body,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendAsync<TResponse>(HttpMethod.Put, relativeUrl, body, cancellationToken, headers);

    public Task PutAsync<TRequest>(
        string relativeUrl,
        TRequest body,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendAsync<TRequest?>(HttpMethod.Put, relativeUrl, body: body, cancellationToken, headers);

    public Task PatchAsync<TRequest>(
        string relativeUrl,
        TRequest body,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendAsync<object?>(HttpMethod.Patch, relativeUrl, body, cancellationToken, headers);

    public Task<TResponse?> PatchAsync<TRequest, TResponse>(
        string relativeUrl,
        TRequest body,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendAsync<TResponse>(HttpMethod.Patch, relativeUrl, body, cancellationToken, headers);

    public Task DeleteAsync(
        string relativeUrl,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendAsync<object?>(HttpMethod.Delete, relativeUrl, body: null, cancellationToken, headers);

    public Task<string?> GetStringAsync(
        string relativeUrl,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendRawStringAsync(HttpMethod.Get, relativeUrl, cancellationToken, headers);

    public Task<byte[]?> GetBytesAsync(
        string relativeUrl,
        CancellationToken cancellationToken = default,
        IDictionary<string, string>? headers = null)
        => SendRawBytesAsync(HttpMethod.Get, relativeUrl, cancellationToken, headers);

    private async Task<TResponse?> SendAsync<TResponse>(
        HttpMethod method,
        string relativeUrl,
        object? body,
        CancellationToken cancellationToken,
        IDictionary<string, string>? headers)
    {
        using var req = new HttpRequestMessage(method, relativeUrl);

        ApplyHeaders(req, headers);

        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, _jsonOptions);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var resp = await _httpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);

        var respJson = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!resp.IsSuccessStatusCode)
            throw new HttpException(respJson ?? string.Empty, resp.StatusCode);

        if (string.IsNullOrWhiteSpace(respJson))
            return default;

        return JsonSerializer.Deserialize<TResponse>(respJson, _jsonOptions);
    }

    private async Task<string?> SendRawStringAsync(
        HttpMethod method,
        string relativeUrl,
        CancellationToken cancellationToken,
        IDictionary<string, string>? headers)
    {
        using var req = new HttpRequestMessage(method, relativeUrl);
        ApplyHeaders(req, headers);

        using var resp = await _httpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);

        var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return !resp.IsSuccessStatusCode ? throw new HttpException(text ?? string.Empty, resp.StatusCode) : text;
    }

    private async Task<byte[]?> SendRawBytesAsync(
        HttpMethod method,
        string relativeUrl,
        CancellationToken cancellationToken,
        IDictionary<string, string>? headers)
    {
        using var req = new HttpRequestMessage(method, relativeUrl);
        ApplyHeaders(req, headers);

        using var resp = await _httpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
        _interceptor?.Intercept(resp);

        var bytes = await resp.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);

        if (resp.IsSuccessStatusCode)
            return bytes;

        var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        throw new HttpException(text, resp.StatusCode);
    }

    private static void ApplyHeaders(HttpRequestMessage req, IDictionary<string, string>? headers)
    {
        if (headers is null || headers.Count == 0)
            return;

        foreach (var (k, v) in headers)
        {
            req.Headers.TryAddWithoutValidation(k, v);
        }
    }
}
