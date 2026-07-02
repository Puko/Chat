using Chat.App.Contract.Errors;
using Chat.App.Wpf.Localization;

namespace Chat.App.Wpf.Services.Http.ErrorMessages;

/// <summary>Error messages for codes returned by login and registration endpoints.</summary>
internal static class AuthErrorMessages
{
    private static readonly Dictionary<string, string> Keys = new(StringComparer.Ordinal)
    {
        [AuthErrorCodes.InvalidCredentials] = "Errors_InvalidCredentials",
        [AuthErrorCodes.Unauthenticated] = "Errors_Unauthenticated",
        [AuthErrorCodes.Forbidden] = "Errors_Forbidden"
    };

    public static bool TryResolve(string errorCode, out string message)
    {
        if (Keys.TryGetValue(errorCode, out var key))
        {
            message = StringLocalizer.Instance[key];
            return true;
        }

        message = null!;
        return false;
    }
}
