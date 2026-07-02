using System.Net.WebSockets;
using Chat.App.Websockets.Abstractions;
using Chat.App.Websockets.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Chat.App.Websockets.Handlers;

public sealed class WebSocketConnectionHandler(
    IWebSocketConnectionManager connectionManager,
    ILogger<WebSocketConnectionHandler> logger)
{
    public async Task HandleAsync(HttpContext context, CancellationToken ct = default)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Expected a WebSocket request.", ct);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized.", ct);
            return;
        }

        WebSocket webSocket;

        try
        {
            webSocket = await context.WebSockets.AcceptWebSocketAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "WebSocket upgrade failed.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return;
        }

        ConnectedClient client;

        try
        {
            client = ConnectedClient.FromClaims(webSocket, context.User);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "WebSocket connection rejected due to invalid JWT claims.");
            await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid token claims.", ct);
            return;
        }

        await connectionManager.HandleConnectionAsync(webSocket, client, ct);
    }
}
