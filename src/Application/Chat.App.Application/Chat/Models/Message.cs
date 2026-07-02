namespace Chat.App.Application.Chat.Models;

public sealed class Message
{
    public Guid Id { get; init; }
    public Guid ConversationId { get; init; }
    public Guid SenderId { get; init; }
    public string SenderUsername { get; set; } = null!;
    public string Content { get; init; } = null!;
    public DateTime SentAtUtc { get; init; }
}
