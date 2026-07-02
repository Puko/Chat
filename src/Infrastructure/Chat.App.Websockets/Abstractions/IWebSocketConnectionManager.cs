using System.Net.WebSockets;
using Chat.App.Websockets.Models;

namespace Chat.App.Websockets.Abstractions;

public interface IWebSocketConnectionManager
{
    Task HandleConnectionAsync(WebSocket webSocket, ConnectedClient client, CancellationToken ct = default);

    Task SendTextToUserAsync(Guid userId, string text, CancellationToken ct = default);
}
