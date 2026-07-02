namespace Chat.App.Database.Entities;

public sealed class ConversationParticipant
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAtUtc { get; set; }
    public DateTime? LastReadAtUtc { get; set; }

    public Conversation Conversation { get; set; } = null!;
    public User User { get; set; } = null!;
}
