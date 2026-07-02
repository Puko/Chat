using Chat.App.Application.Chat.Extensions;
using Chat.App.Application.Chat.Models;
using Chat.App.Application.Common;
using Chat.App.Database.Abstracts;
using MediatR;

namespace Chat.App.Application.Chat.Queries;

public sealed class GetConversationsQueryHandler(
    ICurrentUser currentUser,
    IConversationRepository conversationRepository,
    IMessageRepository messageRepository)
    : IRequestHandler<GetConversationsQuery, IReadOnlyList<ConversationSummary>>
{
    public async Task<IReadOnlyList<ConversationSummary>> Handle(GetConversationsQuery request, CancellationToken ct)
    {
        var userId = currentUser.RequireUserId();
        var conversations = await conversationRepository.GetForUserAsync(userId, ct);

        var result = new List<ConversationSummary>(conversations.Count);

        foreach (var entity in conversations)
        {
            var currentParticipant = entity.Participants.First(participant => participant.UserId == userId);
            var unreadCount = await messageRepository.CountUnreadAsync(
                entity.Id,
                userId,
                currentParticipant.LastReadAtUtc,
                ct);

            result.Add(entity.ToConversationSummary(userId, unreadCount));
        }

        return result;
    }
}
