namespace Chat.App.Contract.Chat;

public sealed class WebSocketClientEnvelopeDto
{
    public string Type { get; set; } = null!;
    public SendMessagePayloadDto? Payload { get; set; }
}
