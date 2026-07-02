namespace Chat.App.Contract.Chat;

public sealed record MessageDto(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    string SenderUsername,
    string Content,
    DateTime SentAtUtc);
