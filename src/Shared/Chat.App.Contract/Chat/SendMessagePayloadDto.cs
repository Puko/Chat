namespace Chat.App.Contract.Chat;

public sealed class SendMessagePayloadDto
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = null!;
}
