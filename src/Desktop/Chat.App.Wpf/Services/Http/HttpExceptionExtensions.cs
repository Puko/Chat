using Chat.App.Contract.Errors;
using ChatApp.Http;

namespace Chat.App.Wpf.Services.Http;

public static class HttpExceptionExtensions
{
    public static ApiProblemDetailsDto? ToProblemDetails(this HttpException exception) =>
        exception.To<ApiProblemDetailsDto>();
}
