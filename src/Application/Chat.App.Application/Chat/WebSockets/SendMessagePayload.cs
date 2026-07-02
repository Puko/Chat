namespace Chat.App.Application.Chat.WebSockets;

public sealed class SendMessagePayload
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = null!;
}
