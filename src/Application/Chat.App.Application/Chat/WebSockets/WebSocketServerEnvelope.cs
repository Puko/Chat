namespace Chat.App.Application.Chat.WebSockets;

public sealed class WebSocketServerEnvelope
{
    public string Type { get; set; } = null!;
    public object? Payload { get; set; }
}
