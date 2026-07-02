using Chat.App.Application.Chat.Models;
using Mapster;
using DbConversation = Chat.App.Database.Entities.Conversation;

namespace Chat.App.Application.Chat.Extensions;

internal static class ConversationExtensions
{
    public static Conversation ToConversation(this DbConversation entity)
    {
        var conversation = entity.Adapt<Conversation>();

        foreach (var participant in conversation.Participants)
        {
            participant.Username = entity.Participants
                .Single(p => p.UserId == participant.UserId)
                .User.Username;
        }

        return conversation;
    }

    public static ConversationSummary ToConversationSummary(
        this DbConversation entity,
        Guid currentUserId,
        int unreadCount)
    {
        var otherParticipant = entity.Participants
            .FirstOrDefault(participant => participant.UserId != currentUserId);

        return new ConversationSummary
        {
            Id = entity.Id,
            Type = (ConversationType)(int)entity.Type,
            LastMessageAtUtc = entity.LastMessageAtUtc,
            LastMessagePreview = entity.Messages.FirstOrDefault()?.Content,
            OtherParticipantId = otherParticipant?.UserId,
            OtherParticipantUsername = otherParticipant?.User.Username,
            UnreadCount = unreadCount
        };
    }
}
