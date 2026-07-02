namespace Chat.App.Contract.Chat;

public sealed record ConversationParticipantDto(
    Guid UserId,
    string Username,
    DateTime? LastReadAtUtc);
