namespace Chat.App.Contract.Chat;

public sealed class WebSocketServerEnvelopeDto
{
    public string Type { get; set; } = null!;
    public object? Payload { get; set; }
}
