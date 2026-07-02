using Chat.App.Contract.Chat;

namespace Chat.App.Wpf.Services;

public interface IWebsocketChatService
{
    event Action<MessageDto>? MessageReceived;

    event Action<string>? ErrorReceived;

    bool IsConnected { get; }

    Task<bool> ConnectAsync(string accessToken, CancellationToken ct = default);

    Task DisconnectAsync();

    Task SendMessageAsync(Guid conversationId, string content, CancellationToken ct = default);
}
