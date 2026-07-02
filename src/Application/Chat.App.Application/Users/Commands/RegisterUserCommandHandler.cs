using Chat.App.Application.Errors;
using Chat.App.Application.Exceptions;
using Chat.App.Application.Users.Models;
using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using DbUser = Chat.App.Database.Entities.User;
using DbUserRole = Chat.App.Database.Entities.UserRole;

namespace Chat.App.Application.Users.Commands;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher<DbUser> passwordHasher)
    : IRequestHandler<RegisterUserCommand, User>
{
    public async Task<User> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var registerUser = request.User;

        var existingUser = await userRepository.GetByUsernameAsync(registerUser.Username, ct);
        if (existingUser is not null)
            throw new ChatAppException(
                UserErrorCodes.DuplicateUsername,
                $"User with username '{registerUser.Username}' already exists.");

        var entity = new DbUser
        {
            Id = Guid.NewGuid(),
            Username = registerUser.Username.Trim(),
            Role = DbUserRole.User,
            CreatedAtUtc = DateTime.UtcNow
        };

        entity.PasswordHash = passwordHasher.HashPassword(entity, registerUser.Password);

        await userRepository.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new User
        {
            Id = entity.Id,
            Username = entity.Username,
            Role = UserRole.User,
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }
}
