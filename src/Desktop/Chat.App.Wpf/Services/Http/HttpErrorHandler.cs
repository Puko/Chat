using System.Net;
using Chat.App.Wpf.Models;
using ChatApp.Http;

namespace Chat.App.Wpf.Services.Http;

public sealed class HttpErrorHandler(
    TokenStorageService tokenStorage,
    AppSession session,
    TokenRefreshService tokenRefresh)
{
    private static readonly IReadOnlyList<ErrorCodeMessageResolver> NoDomainResolvers = [];

    public event Action? Unauthorized;

    public string GetMessage(Exception exception) =>
        ApiErrorMessageResolver.Resolve(exception, NoDomainResolvers);

    public string GetMessage(Exception exception, IReadOnlyList<ErrorCodeMessageResolver> errorResolvers) =>
        ApiErrorMessageResolver.Resolve(exception, errorResolvers);

    public async Task<ServiceResult<T>> ExecuteAsync<T>(
        Func<Task<T>> action,
        IReadOnlyList<ErrorCodeMessageResolver> errorResolvers,
        CancellationToken ct = default)
    {
        try
        {
            return ServiceResult<T>.Ok(await action());
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (HttpException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Unauthorized && await tokenRefresh.TryRefreshAsync(ct))
            {
                try
                {
                    return ServiceResult<T>.Ok(await action());
                }
                catch (HttpException retryEx)
                {
                    return await FailAsync<T>(retryEx, errorResolvers);
                }
            }

            return await FailAsync<T>(ex, errorResolvers);
        }
        catch (Exception ex)
        {
            return ServiceResult<T>.Fail(GetMessage(ex, errorResolvers));
        }
    }

    public async Task<ServiceResult<bool>> ExecuteAsync(
        Func<Task> action,
        IReadOnlyList<ErrorCodeMessageResolver> errorResolvers,
        CancellationToken ct = default)
    {
        var result = await ExecuteAsync(async () =>
        {
            await action();
            return true;
        }, errorResolvers, ct);

        return result.IsSuccess
            ? ServiceResult<bool>.Ok(true)
            : ServiceResult<bool>.Fail(result.Error!);
    }

    private async Task<ServiceResult<T>> FailAsync<T>(
        HttpException ex,
        IReadOnlyList<ErrorCodeMessageResolver> errorResolvers)
    {
        if (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            await tokenStorage.RemoveTokensAsync();
            session.Clear();
            Unauthorized?.Invoke();
        }

        return ServiceResult<T>.Fail(GetMessage(ex, errorResolvers));
    }
}
