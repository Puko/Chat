namespace Chat.App.Contract.Chat;

public sealed class CreateDirectConversationRequestDto
{
    public Guid OtherUserId { get; set; }
}
