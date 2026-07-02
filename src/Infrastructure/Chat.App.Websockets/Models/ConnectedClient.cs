using System.Net.WebSockets;
using System.Security.Claims;

namespace Chat.App.Websockets.Models;

public sealed class ConnectedClient
{
    public required string ConnectionId { get; init; }
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
    public required string Role { get; init; }
    public required WebSocket WebSocket { get; init; }
    public DateTime ConnectedAtUtc { get; init; } = DateTime.UtcNow;

    public static ConnectedClient FromClaims(WebSocket webSocket, ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User id claim is missing.");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidOperationException("User id claim is invalid.");

        var username = user.FindFirstValue(ClaimTypes.Name)
            ?? user.FindFirstValue("unique_name")
            ?? throw new InvalidOperationException("Username claim is missing.");

        var role = user.FindFirstValue(ClaimTypes.Role)
            ?? throw new InvalidOperationException("Role claim is missing.");

        return new ConnectedClient
        {
            ConnectionId = Guid.NewGuid().ToString("N"),
            UserId = userId,
            Username = username,
            Role = role,
            WebSocket = webSocket
        };
    }
}
