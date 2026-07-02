using Chat.App.Contract.Errors;
using Chat.App.Wpf.Localization;

namespace Chat.App.Wpf.Services.Http.ErrorMessages;

/// <summary>Error messages for codes shared across all endpoints.</summary>
internal static class CommonErrorMessages
{
    private static readonly Dictionary<string, string> Keys = new(StringComparer.Ordinal)
    {
        [CommonErrorCodes.MappingFailed] = "Errors_MappingFailed"
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
