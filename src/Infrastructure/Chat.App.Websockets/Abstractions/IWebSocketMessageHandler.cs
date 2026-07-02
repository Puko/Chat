using Chat.App.Websockets.Models;

namespace Chat.App.Websockets.Abstractions;

public interface IWebSocketMessageHandler
{
    Task HandleAsync(ConnectedClient client, string message, CancellationToken ct = default);
}
