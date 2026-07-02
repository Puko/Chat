namespace Chat.App.Contract.Chat;

public sealed record ConversationDto(
    Guid Id,
    ConversationType Type,
    DateTime CreatedAtUtc,
    DateTime? LastMessageAtUtc,
    IReadOnlyList<ConversationParticipantDto> Participants);
