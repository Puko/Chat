using Chat.App.Application.Users;
using Chat.App.Application.Users.Commands;
using Chat.App.Application.Users.Models;
using Chat.App.Contract.Auth;
using Chat.App.Contract.Users;
using Chat.App.JWT;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Chat.App.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class LoginController(
    IMediator mediator,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenIssuer refreshTokenIssuer) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken ct)
    {
        var loginRequest = request.Adapt<LoginRequest>();
        var user = await mediator.Send(new CommandRecords(loginRequest), ct);

        return Ok(await BuildResponseAsync(user, ct));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> Refresh(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken ct)
    {
        var user = await mediator.Send(new RefreshTokenCommand(request.RefreshToken), ct);

        return Ok(await BuildResponseAsync(user, ct));
    }

    private async Task<LoginResponseDto> BuildResponseAsync(User user, CancellationToken ct)
    {
        var accessToken = jwtTokenGenerator.Generate(user.Id, user.Username, user.Role.ToString());
        var refreshToken = await refreshTokenIssuer.IssueAsync(user.Id, ct);

        return new LoginResponseDto(
            accessToken.AccessToken,
            accessToken.ExpiresAtUtc,
            refreshToken.Token,
            user.Adapt<UserDto>());
    }
}
