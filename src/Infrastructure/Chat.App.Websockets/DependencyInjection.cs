using Chat.App.Websockets.Abstractions;
using Chat.App.Websockets.Handlers;
using Chat.App.Websockets.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.App.Websockets;

public static class DependencyInjection
{
    public static IServiceCollection AddWebSockets(this IServiceCollection services)
    {
        services.AddSingleton<IWebSocketConnectionRegistry, WebSocketConnectionRegistry>();
        services.AddSingleton<IWebSocketConnectionManager, WebSocketConnectionManager>();
        services.AddScoped<WebSocketConnectionHandler>();

        return services;
    }
}
