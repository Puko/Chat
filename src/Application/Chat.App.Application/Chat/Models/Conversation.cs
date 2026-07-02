namespace Chat.App.Application.Chat.Models;

public sealed class Conversation
{
    public Guid Id { get; init; }
    public ConversationType Type { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? LastMessageAtUtc { get; init; }
    public IReadOnlyList<ConversationParticipant> Participants { get; init; } = [];
}
