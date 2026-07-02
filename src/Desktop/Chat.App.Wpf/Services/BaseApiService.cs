using Chat.App.Wpf.Services.Http;
using ChatApp.Http;
using Flurl;

namespace Chat.App.Wpf.Services;

public abstract class BaseApiService(
    JsonHttpClient httpClient,
    TokenStorageService tokenStorage,
    HttpErrorHandler errorHandler)
{
    protected JsonHttpClient Http { get; } = httpClient;

    protected TokenStorageService TokenStorage { get; } = tokenStorage;
    protected virtual IReadOnlyList<ErrorCodeMessageResolver> ErrorResolvers { get; } = [];

    protected static string AppendToQuery<T>(string action, T? query)
    {
        if (query is null)
            return action;

        return action.SetQueryParams(query).ToString();
    }

    protected Task<ServiceResult<T>> ExecuteAsync<T>(
        Func<Task<T>> action,
        CancellationToken ct = default)
        => errorHandler.ExecuteAsync(action, ErrorResolvers, ct);

    protected Task<ServiceResult<bool>> ExecuteAsync(
        Func<Task> action,
        CancellationToken ct = default)
        => errorHandler.ExecuteAsync(action, ErrorResolvers, ct);

    protected async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
        => await Http.GetAsync<T>(url, ct, await GetAuthHeadersAsync());

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default)
        => await Http.PostAsync<TRequest, TResponse>(url, body, ct, await GetAuthHeadersAsync());

    protected async Task<TResponse?> PostAsync<TResponse>(string url, CancellationToken ct = default)
        => await Http.PostAsync<TResponse>(url, ct, await GetAuthHeadersAsync());

    private async Task<IDictionary<string, string>?> GetAuthHeadersAsync()
    {
        var token = await TokenStorage.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        return new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {token}"
        };
    }
}
