namespace Chat.App.Contract.Errors;

public static class ChatErrorCodes
{
    public const string ConversationNotFound = "chat.conversation_not_found";
    public const string NotParticipant = "chat.not_participant";
    public const string UserNotFound = "chat.user_not_found";
    public const string CannotChatWithSelf = "chat.cannot_chat_with_self";
}
