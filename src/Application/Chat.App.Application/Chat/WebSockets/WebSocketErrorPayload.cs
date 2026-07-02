namespace Chat.App.Application.Chat.WebSockets;

public sealed class WebSocketErrorPayload
{
    public string ErrorCode { get; set; } = null!;
    public string Message { get; set; } = null!;
}
