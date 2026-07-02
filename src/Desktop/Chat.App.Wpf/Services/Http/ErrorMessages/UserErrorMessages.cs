using Chat.App.Contract.Errors;
using Chat.App.Wpf.Localization;

namespace Chat.App.Wpf.Services.Http.ErrorMessages;

/// <summary>Error messages for codes returned by user endpoints (e.g. registration, search).</summary>
internal static class UserErrorMessages
{
    private static readonly Dictionary<string, string> Keys = new(StringComparer.Ordinal)
    {
        [UserErrorCodes.DuplicateUsername] = "Errors_DuplicateUsername"
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
