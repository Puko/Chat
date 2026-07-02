namespace Chat.App.Contract.Chat;

public sealed record ConversationSummaryDto(
    Guid Id,
    ConversationType Type,
    DateTime? LastMessageAtUtc,
    string? LastMessagePreview,
    Guid? OtherParticipantId,
    string? OtherParticipantUsername,
    int UnreadCount);
