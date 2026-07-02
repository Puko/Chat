using Chat.App.Contract.Errors;
using Chat.App.Wpf.Localization;

namespace Chat.App.Wpf.Services.Http.ErrorMessages;

/// <summary>Error messages for codes returned by conversation and messaging endpoints.</summary>
internal static class ChatErrorMessages
{
    private static readonly Dictionary<string, string> Keys = new(StringComparer.Ordinal)
    {
        [ChatErrorCodes.ConversationNotFound] = "Errors_ConversationNotFound",
        [ChatErrorCodes.NotParticipant] = "Errors_NotParticipant",
        [ChatErrorCodes.UserNotFound] = "Errors_UserNotFound",
        [ChatErrorCodes.CannotChatWithSelf] = "Errors_CannotChatWithSelf"
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
