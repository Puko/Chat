using Chat.App.Contract.Chat;

namespace Chat.App.Wpf.Services.Websocket;

public sealed class WebsocketChatService : IWebsocketChatService, IAsyncDisposable
{
    private readonly ChatWebSocketClient _webSocket = new();
    private readonly string _apiBaseUrl;

    public WebsocketChatService(string apiBaseUrl)
    {
        _apiBaseUrl = apiBaseUrl;
        _webSocket.MessageReceived += message => MessageReceived?.Invoke(message);
        _webSocket.ErrorReceived += error => ErrorReceived?.Invoke(error);
    }

    public event Action<MessageDto>? MessageReceived;

    public event Action<string>? ErrorReceived;

    public bool IsConnected => _webSocket.IsConnected;

    public Task<bool> ConnectAsync(string accessToken, CancellationToken ct = default)
        => _webSocket.ConnectAsync(_apiBaseUrl, accessToken, ct);

    public Task DisconnectAsync()
        => _webSocket.DisconnectAsync();

    public Task SendMessageAsync(Guid conversationId, string content, CancellationToken ct = default)
        => _webSocket.SendMessageAsync(conversationId, content, ct);

    public ValueTask DisposeAsync()
        => _webSocket.DisposeAsync();
}
