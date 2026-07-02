using Chat.App.Application.Chat.Extensions;
using Chat.App.Application.Chat.Models;
using Chat.App.Application.Common;
using Chat.App.Application.Errors;
using Chat.App.Application.Exceptions;
using Chat.App.Application.Paging;
using Chat.App.Database.Abstracts;
using MediatR;

namespace Chat.App.Application.Chat.Queries;

public sealed class GetMessagesQueryHandler(
    ICurrentUser currentUser,
    IConversationRepository conversationRepository,
    IMessageRepository messageRepository)
    : IRequestHandler<GetMessagesQuery, PagedResult<Message>>
{
    public async Task<Paging.PagedResult<Message>> Handle(GetMessagesQuery request, CancellationToken ct)
    {
        var userId = currentUser.RequireUserId();

        if (!await conversationRepository.IsParticipantAsync(request.ConversationId, userId, ct))
            throw new ChatAppException(ChatErrorCodes.NotParticipant, "You are not a participant of this conversation.");

        var (entities, totalCount) = await messageRepository.GetForConversationAsync(
            request.ConversationId,
            request.PageNumber,
            request.PageSize,
            ct);

        return new Paging.PagedResult<Message>
        {
            Items = entities.Select(MessageExtensions.ToMessage).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
