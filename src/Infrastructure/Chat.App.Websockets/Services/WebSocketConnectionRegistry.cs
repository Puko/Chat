using System.Collections.Concurrent;
using Chat.App.Websockets.Abstractions;
using Chat.App.Websockets.Models;

namespace Chat.App.Websockets.Services;

public sealed class WebSocketConnectionRegistry : IWebSocketConnectionRegistry
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, ConnectedClient>> _connections = new();

    public int Count => _connections.Values.Sum(group => group.Count);

    public void Add(ConnectedClient client)
    {
        var userConnections = _connections.GetOrAdd(client.UserId, _ => new ConcurrentDictionary<string, ConnectedClient>());
        userConnections[client.ConnectionId] = client;
    }

    public bool Remove(Guid userId, string connectionId)
    {
        if (!_connections.TryGetValue(userId, out var userConnections))
            return false;

        if (!userConnections.TryRemove(connectionId, out _))
            return false;

        if (userConnections.IsEmpty)
            _connections.TryRemove(userId, out _);

        return true;
    }

    public IReadOnlyCollection<ConnectedClient> GetByUserId(Guid userId)
    {
        if (!_connections.TryGetValue(userId, out var userConnections))
            return [];

        return userConnections.Values.ToArray();
    }

    public IReadOnlyCollection<ConnectedClient> GetAll()
        => _connections.Values.SelectMany(group => group.Values).ToArray();
}
