using Chat.App.Application.Users.Commands;
using Chat.App.Contract.Auth;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RegisterUser = Chat.App.Application.Users.Models.RegisterUser;
using RegisterUserDto = Chat.App.Contract.Auth.RegisterUser;

namespace Chat.App.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserResponseDto>> Register(
        [FromBody] RegisterUserDto request,
        CancellationToken ct)
    {
        var command = new RegisterUserCommand(request.Adapt<RegisterUser>());
        var result = await mediator.Send(command, ct);

        return Created($"/api/users/{result.Id}", result.Adapt<RegisterUserResponseDto>());
    }
}
