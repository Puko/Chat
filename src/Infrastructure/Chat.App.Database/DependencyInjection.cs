using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstraction.Abstracts;
using Chat.App.Database.Abstracts;
using Chat.App.Database.Contexts;
using Chat.App.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.App.Database;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {

        services.AddDbContext<ChatAppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            //options.UseNpgsql(connectionString);
        });
        return services.AddChatAppPersistence();
    }

    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configure)
    {
        services.AddDbContext<ChatAppDbContext>(configure);
        return services.AddChatAppPersistence();
    }

    public static IServiceCollection AddChatAppPersistence(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ChatAppDbContext>());
        services.AddScoped<IDbContextAccessor>(sp => sp.GetRequiredService<ChatAppDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}
