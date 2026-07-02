using System.Text.Json;
using Chat.App.Application.Chat.Commands;
using Chat.App.Application.Common;
using Chat.App.Application.Exceptions;
using Chat.App.Websockets.Abstractions;
using Chat.App.Websockets.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.App.Application.Chat.WebSockets;

public sealed class ChatWebSocketService(
    IMediator mediator,
    IWebSocketConnectionManager connectionManager,
    ICurrentUserInitializer currentUserInitializer,
    ILogger<ChatWebSocketService> logger)
    : IWebSocketMessageHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task HandleAsync(ConnectedClient client, string message, CancellationToken ct = default)
    {
        currentUserInitializer.SetUserId(client.UserId);

        WebSocketClientEnvelope? envelope;

        try
        {
            envelope = JsonSerializer.Deserialize<WebSocketClientEnvelope>(message, JsonOptions);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Invalid WebSocket JSON from UserId={UserId}", client.UserId);
            await SendErrorAsync(client.UserId, "invalid_payload", "Invalid message format.", ct);
            return;
        }

        if (envelope is null || string.IsNullOrWhiteSpace(envelope.Type))
        {
            await SendErrorAsync(client.UserId, "invalid_payload", "Message type is required.", ct);
            return;
        }

        try
        {
            switch (envelope.Type)
            {
                case WebSocketMessageTypes.SendMessage:
                    await HandleSendMessageAsync(envelope.Payload, ct);
                    break;

                default:
                    await SendErrorAsync(client.UserId, "unsupported_message_type", $"Unsupported message type '{envelope.Type}'.", ct);
                    break;
            }
        }
        catch (ChatAppException ex)
        {
            await SendErrorAsync(client.UserId, ex.ErrorCode, ex.Message, ct);
        }
    }

    private async Task HandleSendMessageAsync(SendMessagePayload? payload, CancellationToken ct)
    {
        if (payload is null || payload.ConversationId == Guid.Empty || string.IsNullOrWhiteSpace(payload.Content))
            throw new ChatAppException("invalid_payload", "ConversationId and content are required.");

        await mediator.Send(new SendMessageCommand(payload.ConversationId, payload.Content), ct);
    }

    private Task SendErrorAsync(Guid userId, string errorCode, string message, CancellationToken ct)
    {
        var envelope = new WebSocketServerEnvelope
        {
            Type = WebSocketMessageTypes.Error,
            Payload = new WebSocketErrorPayload
            {
                ErrorCode = errorCode,
                Message = message
            }
        };

        var json = JsonSerializer.Serialize(envelope, JsonOptions);
        return connectionManager.SendTextToUserAsync(userId, json, ct);
    }
}
