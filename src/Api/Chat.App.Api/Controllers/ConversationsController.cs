using Chat.App.Application.Chat.Commands;
using Chat.App.Application.Chat.Queries;
using Chat.App.Contract.Chat;
using Chat.App.Contract.Paging;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContractConversationDto = Chat.App.Contract.Chat.ConversationDto;
using ContractConversationSummaryDto = Chat.App.Contract.Chat.ConversationSummaryDto;
using ContractMessageDto = Chat.App.Contract.Chat.MessageDto;

namespace Chat.App.Api.Controllers;

[ApiController]
[Route("api/conversations")]
[Authorize]
public sealed class ConversationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContractConversationSummaryDto>>> GetConversations(CancellationToken ct)
    {
        var conversations = await mediator.Send(new GetConversationsQuery(), ct);
        return Ok(conversations.Adapt<List<ContractConversationSummaryDto>>());
    }

    [HttpPost("direct")]
    public async Task<ActionResult<ContractConversationDto>> CreateDirect(
        [FromBody] CreateDirectConversationRequestDto request,
        CancellationToken ct)
    {
        var conversation = await mediator.Send(
            new GetOrCreateDirectConversationCommand(request.OtherUserId),
            ct);

        return Ok(conversation.Adapt<ContractConversationDto>());
    }

    [HttpGet("{conversationId:guid}/messages")]
    public async Task<ActionResult<PagedResultDto<ContractMessageDto>>> GetMessages(
        Guid conversationId,
        [FromQuery] GetMessagesRequestDto request,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetMessagesQuery(
                conversationId,
                request.PageNumber,
                request.PageSize),
            ct);

        return Ok(result.Adapt<PagedResultDto<ContractMessageDto>>());
    }

    [HttpPatch("{conversationId:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid conversationId, CancellationToken ct)
    {
        await mediator.Send(new MarkConversationReadCommand(conversationId), ct);
        return NoContent();
    }
}
