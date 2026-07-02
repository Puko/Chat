using Chat.App.Application.Chat.Models;
using Chat.App.Application.Paging;
using MediatR;

namespace Chat.App.Application.Chat.Queries;

public sealed record GetConversationsQuery : IRequest<IReadOnlyList<ConversationSummary>>;

public sealed record GetMessagesQuery(
    Guid ConversationId,
    int PageNumber,
    int PageSize) : IRequest<PagedResult<Message>>;
