using Chat.App.Application.Exceptions;
using Chat.App.Contract.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Chat.App.Api.Infrastructure;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Request failed.");

        if (exception is ValidationException validationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problemDetails = new ValidationProblemDetails(
                validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray()))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed."
            };

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
        }

        if (exception is ChatAppException chatAppException)
        {
            var statusCode = chatAppException.ErrorCode switch
            {
                UserErrorCodes.DuplicateUsername => StatusCodes.Status409Conflict,
                AuthErrorCodes.InvalidCredentials => StatusCodes.Status401Unauthorized,
                AuthErrorCodes.InvalidRefreshToken => StatusCodes.Status401Unauthorized,
                CommonErrorCodes.MappingFailed => StatusCodes.Status500InternalServerError,
                ChatErrorCodes.ConversationNotFound => StatusCodes.Status404NotFound,
                ChatErrorCodes.UserNotFound => StatusCodes.Status404NotFound,
                ChatErrorCodes.NotParticipant => StatusCodes.Status403Forbidden,
                ChatErrorCodes.CannotChatWithSelf => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };

            httpContext.Response.StatusCode = statusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Application error.",
                    Detail = chatAppException.Message,
                    Extensions = { ["errorCode"] = chatAppException.ErrorCode }
                }
            });
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred."
            }
        });
    }
}
