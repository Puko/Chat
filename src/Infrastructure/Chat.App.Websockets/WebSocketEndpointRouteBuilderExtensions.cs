using Chat.App.Websockets.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Chat.App.Websockets;

public static class WebSocketEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapChatWebSocket(this IEndpointRouteBuilder endpoints, string pattern = "/ws")
    {
        endpoints.Map(pattern, async (HttpContext context, WebSocketConnectionHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(context, ct);
        }).RequireAuthorization();

        return endpoints;
    }
}
