using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Chat.App.Contract.Chat;
using Chat.App.Wpf.Localization;
using ChatApp.Http;

namespace Chat.App.Wpf.Services;

public sealed class ChatWebSocketClient : IAsyncDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = JsonOptions.Value;

    private sealed record Connection(ClientWebSocket Socket, CancellationTokenSource Cts, Task ListenTask);

    private Connection? _connection;

    public event Action<MessageDto>? MessageReceived;

    public event Action<string>? ErrorReceived;

    public bool IsConnected => _connection?.Socket.State == WebSocketState.Open;

    public async Task<bool> ConnectAsync(string apiBaseUrl, string accessToken, CancellationToken ct = default)
    {
        await DisconnectAsync();

        var socket = new ClientWebSocket();
        var wsUrl = ToWebSocketUrl(apiBaseUrl) + $"/ws?access_token={Uri.EscapeDataString(accessToken)}";

        try
        {
            await socket.ConnectAsync(new Uri(wsUrl), ct);
        }
        catch (Exception) when (!ct.IsCancellationRequested)
        {
            socket.Dispose();
            ErrorReceived?.Invoke(StringLocalizer.Instance["Errors_WebSocketConnectFailed"]);
            return false;
        }

        var cts = new CancellationTokenSource();
        _connection = new Connection(socket, cts, ListenAsync(socket, cts.Token));
        return true;
    }

    public async Task SendMessageAsync(Guid conversationId, string content, CancellationToken ct = default)
    {
        if (_connection is not { Socket.State: WebSocketState.Open } connection)
            throw new InvalidOperationException("WebSocket is not connected.");

        var envelope = new WebSocketClientEnvelopeDto
        {
            Type = WebSocketMessageTypes.SendMessage,
            Payload = new SendMessagePayloadDto
            {
                ConversationId = conversationId,
                Content = content
            }
        };

        var json = JsonSerializer.Serialize(envelope, SerializerOptions);
        var bytes = Encoding.UTF8.GetBytes(json);
        await connection.Socket.SendAsync(bytes, WebSocketMessageType.Text, endOfMessage: true, ct);
    }

    public async Task DisconnectAsync()
    {
        if (_connection is not { } connection)
            return;

        _connection = null;

        await connection.Cts.CancelAsync();
        await SuppressCancellationAsync(connection.ListenTask);

        if (connection.Socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
        {
            try
            {
                await connection.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            catch
            {
            }
        }

        connection.Cts.Dispose();
        connection.Socket.Dispose();
    }

    public async ValueTask DisposeAsync()
        => await DisconnectAsync();

    private async Task ListenAsync(ClientWebSocket socket, CancellationToken ct)
    {
        var buffer = new byte[8192];
        var builder = new StringBuilder();

        try
        {
            while (!ct.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                builder.Clear();
                WebSocketReceiveResult result;

                do
                {
                    result = await socket.ReceiveAsync(buffer, ct);

                    if (result.MessageType == WebSocketMessageType.Close)
                        return;

                    builder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                while (!result.EndOfMessage);

                HandleIncoming(builder.ToString());
            }
        }
        catch (Exception) when (!ct.IsCancellationRequested)
        {
            ErrorReceived?.Invoke(StringLocalizer.Instance["Errors_WebSocketDisconnected"]);
        }
    }

    private static async Task SuppressCancellationAsync(Task task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void HandleIncoming(string json)
    {
        WebSocketServerEnvelopeDto? envelope;

        try
        {
            envelope = JsonSerializer.Deserialize<WebSocketServerEnvelopeDto>(json, SerializerOptions);
        }
        catch
        {
            return;
        }

        if (envelope is null)
            return;

        switch (envelope.Type)
        {
            case WebSocketMessageTypes.MessageReceived:
                var message = DeserializePayload<MessageDto>(envelope.Payload);
                if (message is not null)
                    MessageReceived?.Invoke(message);
                break;

            case WebSocketMessageTypes.Error:
                var error = DeserializePayload<WebSocketErrorPayloadDto>(envelope.Payload);
                if (error is not null)
                    ErrorReceived?.Invoke(error.Message);
                break;
        }
    }

    private static T? DeserializePayload<T>(object? payload)
    {
        if (payload is null)
            return default;

        if (payload is JsonElement element)
            return element.Deserialize<T>(SerializerOptions);

        var json = JsonSerializer.Serialize(payload, SerializerOptions);
        return JsonSerializer.Deserialize<T>(json, SerializerOptions);
    }

    private static string ToWebSocketUrl(string apiBaseUrl)
    {
        var uri = new Uri(apiBaseUrl.TrimEnd('/'));
        var scheme = uri.Scheme switch
        {
            "https" => "wss",
            "http" => "ws",
            _ => uri.Scheme
        };

        return $"{scheme}://{uri.Authority}";
    }
}
