using Chat.App.Application.Errors;
using Chat.App.Application.Exceptions;
using Chat.App.Application.Users;
using Chat.App.Application.Users.Models;

namespace Chat.App.Application.Common;

public static class CurrentUserExtensions
{
    public static Guid RequireUserId(this ICurrentUser currentUser) =>
        currentUser.UserId
        ?? throw new ChatAppException(AuthErrorCodes.Unauthenticated, "User is not authenticated.");

    public static void RequireAdmin(this ICurrentUser currentUser)
    {
        if (!currentUser.IsInRole(Roles.Admin))
            throw new ChatAppException(AuthErrorCodes.Forbidden, "Admin role is required.");
    }
}
