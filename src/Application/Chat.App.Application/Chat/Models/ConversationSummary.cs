namespace Chat.App.Application.Chat.Models;

public sealed class ConversationSummary
{
    public Guid Id { get; init; }
    public ConversationType Type { get; init; }
    public DateTime? LastMessageAtUtc { get; init; }
    public string? LastMessagePreview { get; init; }
    public Guid? OtherParticipantId { get; init; }
    public string? OtherParticipantUsername { get; init; }
    public int UnreadCount { get; init; }
}
