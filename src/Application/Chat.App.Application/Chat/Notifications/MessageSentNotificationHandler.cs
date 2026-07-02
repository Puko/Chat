using System.Text.Json;
using Chat.App.Application.Chat.WebSockets;
using Chat.App.Websockets.Abstractions;
using MediatR;

namespace Chat.App.Application.Chat.Notifications;

public sealed class MessageSentNotificationHandler(IWebSocketConnectionManager connectionManager)
    : INotificationHandler<MessageSentNotification>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task Handle(MessageSentNotification notification, CancellationToken ct)
    {
        var envelope = new WebSocketServerEnvelope
        {
            Type = WebSocketMessageTypes.MessageReceived,
            Payload = notification.Message
        };

        var json = JsonSerializer.Serialize(envelope, JsonOptions);

        foreach (var userId in notification.ParticipantUserIds)
            await connectionManager.SendTextToUserAsync(userId, json, ct);
    }
}
