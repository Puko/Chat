namespace Chat.App.Application.Chat.WebSockets;

public sealed class WebSocketClientEnvelope
{
    public string Type { get; set; } = null!;
    public SendMessagePayload? Payload { get; set; }
}
