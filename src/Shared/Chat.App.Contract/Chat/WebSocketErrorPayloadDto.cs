namespace Chat.App.Contract.Chat;

public sealed class WebSocketErrorPayloadDto
{
    public string ErrorCode { get; set; } = null!;
    public string Message { get; set; } = null!;
}
