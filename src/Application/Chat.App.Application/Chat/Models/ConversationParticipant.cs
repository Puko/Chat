namespace Chat.App.Application.Chat.Models;

public sealed class ConversationParticipant
{
    public Guid UserId { get; init; }
    public string Username { get; set; } = null!;
    public DateTime? LastReadAtUtc { get; init; }
}
