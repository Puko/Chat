using Chat.App.Application.Errors;
using Chat.App.Application.Exceptions;
using Chat.App.Application.Users.Models;
using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstracts;
using Chat.App.JWT;
using MediatR;
using DbUserRole = Chat.App.Database.Entities.UserRole;

namespace Chat.App.Application.Users.Commands;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IRefreshTokenGenerator refreshTokenGenerator)
    : IRequestHandler<RefreshTokenCommand, User>
{
    public async Task<User> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var tokenHash = refreshTokenGenerator.Hash(request.RefreshToken);
        var storedToken = await refreshTokenRepository.GetActiveByHashAsync(tokenHash, ct)
            ?? throw new ChatAppException(AuthErrorCodes.InvalidRefreshToken, "Invalid or expired refresh token.");

        var entity = await userRepository.GetByIdAsync(ct, storedToken.UserId)
            ?? throw new ChatAppException(AuthErrorCodes.InvalidRefreshToken, "Invalid or expired refresh token.");

        storedToken.RevokedAtUtc = DateTime.UtcNow;
        refreshTokenRepository.Update(storedToken);

        await unitOfWork.SaveChangesAsync(ct);

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
