using Chat.App.Websockets.Models;

namespace Chat.App.Websockets.Abstractions;

public interface IWebSocketConnectionRegistry
{
    void Add(ConnectedClient client);

    bool Remove(Guid userId, string connectionId);

    IReadOnlyCollection<ConnectedClient> GetByUserId(Guid userId);

    IReadOnlyCollection<ConnectedClient> GetAll();

    int Count { get; }
}
