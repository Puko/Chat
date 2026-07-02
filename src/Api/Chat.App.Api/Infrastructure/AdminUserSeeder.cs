using Chat.App.Database.Abstracts;
using Chat.App.Database.Abstraction;
using Chat.App.Database.Entities;
using Microsoft.AspNetCore.Identity;

namespace Chat.App.Api.Infrastructure;

public static class AdminUserSeeder
{
    private const string AdminUsername = "admin";
    private const string AdminPassword = "Admin123!";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var existingAdmin = await userRepository.GetByUsernameAsync(AdminUsername, ct);
        if (existingAdmin is not null)
            return;

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Username = AdminUsername,
            Role = UserRole.Admin,
            CreatedAtUtc = DateTime.UtcNow
        };

        admin.PasswordHash = passwordHasher.HashPassword(admin, AdminPassword);

        await userRepository.AddAsync(admin, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
