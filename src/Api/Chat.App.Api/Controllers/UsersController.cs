using Chat.App.Application.Paging;
using Chat.App.Application.Users.Queries;
using Chat.App.Contract.Paging;
using Chat.App.Contract.Users;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.App.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<PagedResultDto<UserDto>>> GetUsers(
        [FromQuery] GetUsersRequestDto request,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetUsersQuery(new Page
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        }, request.Search), ct);
        
        return Ok(result.Adapt<PagedResultDto<UserDto>>());
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResultDto<UserLookupDto>>> SearchUsers(
        [FromQuery] SearchUsersRequestDto request,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new SearchUsersQuery(
                new Page
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                },
                request.Search),
            ct);

        return Ok(result.Adapt<PagedResultDto<UserLookupDto>>());
    }
}
