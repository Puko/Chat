using Chat.App.Wpf.Localization;
using Chat.App.Wpf.Services.Http.ErrorMessages;
using ChatApp.Http;

namespace Chat.App.Wpf.Services.Http;

public static class ApiErrorMessageResolver
{
    public static string Resolve(HttpException exception, IReadOnlyList<ErrorCodeMessageResolver> resolvers)
    {
        var details = exception.ToProblemDetails();
        if (details is null)
            return ResolveStatusCode(exception.StatusCode);

        if (!string.IsNullOrWhiteSpace(details.ErrorCode) && TryResolveErrorMessage(details.ErrorCode, resolvers, out var message))
            return message;

        if (details.Errors is { Count: > 0 })
        {
            var validationMessages = details.Errors
                .SelectMany(pair => pair.Value)
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .ToArray();

            if (validationMessages.Length > 0)
                return string.Join(" ", validationMessages);
        }

        if (!string.IsNullOrWhiteSpace(details.Detail))
            return details.Detail;

        if (!string.IsNullOrWhiteSpace(details.Title))
            return details.Title;

        return ResolveStatusCode(exception.StatusCode);
    }

    public static string Resolve(Exception exception, IReadOnlyList<ErrorCodeMessageResolver> resolvers) =>
        exception is HttpException httpException
            ? Resolve(httpException, resolvers)
            : exception.Message;

    private static bool TryResolveErrorMessage(
        string errorCode,
        IReadOnlyList<ErrorCodeMessageResolver> resolvers,
        out string message)
    {
        foreach (var resolve in resolvers)
        {
            if (resolve(errorCode, out message!))
                return true;
        }

        return CommonErrorMessages.TryResolve(errorCode, out message!);
    }

    private static string ResolveStatusCode(System.Net.HttpStatusCode statusCode) =>
        StringLocalizer.Instance[statusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "Errors_InvalidCredentials",
            System.Net.HttpStatusCode.Forbidden => "Errors_Forbidden",
            System.Net.HttpStatusCode.NotFound => "Errors_NotFound",
            System.Net.HttpStatusCode.Conflict => "Errors_Conflict",
            _ => "Errors_Generic"
        }];
}
