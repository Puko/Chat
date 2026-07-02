using System.Net.WebSockets;
using System.Text;
using Chat.App.Websockets.Abstractions;
using Chat.App.Websockets.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chat.App.Websockets.Services;

public sealed class WebSocketConnectionManager(
    IWebSocketConnectionRegistry registry,
    IServiceScopeFactory scopeFactory,
    ILogger<WebSocketConnectionManager> logger)
    : IWebSocketConnectionManager
{
    private const int BufferSize = 4 * 1024;

    public async Task HandleConnectionAsync(WebSocket webSocket, ConnectedClient client, CancellationToken ct = default)
    {
        registry.Add(client);

        logger.LogInformation(
            "WebSocket connected. UserId={UserId}, ConnectionId={ConnectionId}, Total={Total}",
            client.UserId,
            client.ConnectionId,
            registry.Count);

        var buffer = new byte[BufferSize];

        try
        {
            while (webSocket.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                using var messageStream = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await webSocket.ReceiveAsync(buffer, ct);

                    if (result.MessageType == WebSocketMessageType.Close)
                        return;

                    if (result.Count > 0)
                        messageStream.Write(buffer, 0, result.Count);
                }
                while (!result.EndOfMessage);

                if (result.MessageType != WebSocketMessageType.Text)
                    continue;

                var message = Encoding.UTF8.GetString(messageStream.ToArray());

                await using var scope = scopeFactory.CreateAsyncScope();
                var handler = scope.ServiceProvider.GetRequiredService<IWebSocketMessageHandler>();
                await handler.HandleAsync(client, message, ct);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {

        }
        catch (WebSocketException ex)
        {
            logger.LogWarning(
                ex,
                "WebSocket fault. UserId={UserId}, ConnectionId={ConnectionId}",
                client.UserId,
                client.ConnectionId);
        }
        finally
        {
            registry.Remove(client.UserId, client.ConnectionId);

            if (webSocket.State is WebSocketState.Open or WebSocketState.CloseReceived)
            {
                try
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed.", CancellationToken.None);
                }
                catch (WebSocketException ex)
                {
                    logger.LogDebug(ex, "WebSocket close failed. ConnectionId={ConnectionId}", client.ConnectionId);
                }
            }

            logger.LogInformation(
                "WebSocket disconnected. UserId={UserId}, ConnectionId={ConnectionId}, Total={Total}",
                client.UserId,
                client.ConnectionId,
                registry.Count);
        }
    }

    public async Task SendTextToUserAsync(Guid userId, string text, CancellationToken ct = default)
    {
        var bytes = Encoding.UTF8.GetBytes(text);

        foreach (var client in registry.GetByUserId(userId))
            await SendAsync(client.WebSocket, bytes, ct);
    }

    private static async Task SendAsync(WebSocket webSocket, byte[] payload, CancellationToken ct)
    {
        if (webSocket.State != WebSocketState.Open)
            return;

        await webSocket.SendAsync(payload, WebSocketMessageType.Text, endOfMessage: true, ct);
    }
}
