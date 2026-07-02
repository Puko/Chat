namespace Chat.App.Database.Entities;

public sealed class Conversation
{
    public Guid Id { get; set; }
    public ConversationType Type { get; set; } = ConversationType.Direct;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastMessageAtUtc { get; set; }

    public ICollection<ConversationParticipant> Participants { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
}
