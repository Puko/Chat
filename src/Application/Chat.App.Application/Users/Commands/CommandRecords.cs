using Chat.App.Application.Users.Models;
using MediatR;

namespace Chat.App.Application.Users.Commands;

public sealed record CommandRecords(LoginRequest Request) : IRequest<User>;
public sealed record RegisterUserCommand(RegisterUser User) : IRequest<User>;
public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<User>;
