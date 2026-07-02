namespace Chat.App.Application.Chat.WebSockets;

public static class WebSocketMessageTypes
{
    public const string SendMessage = "send_message";
    public const string MessageReceived = "message.received";
    public const string Error = "error";
}
