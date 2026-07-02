using Chat.App.Application.Behaviors;
using Chat.App.Application.Chat.WebSockets;
using Chat.App.Application.Users;
using Chat.App.Websockets.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.App.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(applicationAssembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IWebSocketMessageHandler, ChatWebSocketService>();
        services.AddScoped<IRefreshTokenIssuer, RefreshTokenIssuer>();

        return services;
    }
}
