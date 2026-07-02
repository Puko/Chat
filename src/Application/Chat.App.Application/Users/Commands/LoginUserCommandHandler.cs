using Chat.App.Application.Errors;
using Chat.App.Application.Exceptions;
using Chat.App.Application.Users.Models;
using Chat.App.Database.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using DbUser = Chat.App.Database.Entities.User;
using DbUserRole = Chat.App.Database.Entities.UserRole;

namespace Chat.App.Application.Users.Commands;

public sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<DbUser> passwordHasher)
    : IRequestHandler<CommandRecords, User>
{
    public async Task<User> Handle(CommandRecords request, CancellationToken ct)
    {
        var loginUser = request.Request;

        var entity = await userRepository.GetByUsernameAsync(loginUser.Username, ct);
        if (entity is null)
            throw new ChatAppException(
                AuthErrorCodes.InvalidCredentials,
                "Invalid username or password.");

        var verification = passwordHasher.VerifyHashedPassword(entity, entity.PasswordHash, loginUser.Password);
        if (verification is PasswordVerificationResult.Failed)
            throw new ChatAppException(
                AuthErrorCodes.InvalidCredentials,
                "Invalid username or password.");

        return new User
        {
            Id = entity.Id,
            Username = entity.Username,
            Role = MapRole(entity.Role),
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }

    private static UserRole MapRole(DbUserRole role) =>
        role switch
        {
            DbUserRole.Admin => UserRole.Admin,
            _ => UserRole.User
        };
}
