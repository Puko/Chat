using Chat.App.Database.Abstracts;
using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstraction.Abstracts;
using Chat.App.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.App.Database.Repositories;

public sealed class ConversationRepository(IDbContextAccessor dbContextAccessor)
    : GenericRepository<Conversation>(dbContextAccessor), IConversationRepository
{
    public async Task<Conversation?> FindDirectConversationAsync(Guid userId1, Guid userId2, CancellationToken ct = default)
    {
        var sharedConversationIds = await (
            from participant1 in Accessor.Set<ConversationParticipant>()
            join participant2 in Accessor.Set<ConversationParticipant>()
                on participant1.ConversationId equals participant2.ConversationId
            where participant1.UserId == userId1 && participant2.UserId == userId2
            select participant1.ConversationId).Distinct().ToListAsync(ct);

        if (sharedConversationIds.Count == 0)
            return null;

        return await Accessor.Set<Conversation>()
            .Include(conversation => conversation.Participants)
            .ThenInclude(participant => participant.User)
            .Where(conversation =>
                sharedConversationIds.Contains(conversation.Id) &&
                conversation.Type == ConversationType.Direct)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Conversation?> GetByIdForUserAsync(Guid conversationId, Guid userId, CancellationToken ct = default)
        => await Accessor.Set<Conversation>()
            .Include(conversation => conversation.Participants)
            .ThenInclude(participant => participant.User)
            .Where(conversation =>
                conversation.Id == conversationId &&
                conversation.Participants.Any(participant => participant.UserId == userId))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId, CancellationToken ct = default)
        => await Accessor.Set<Conversation>()
            .AsNoTracking()
            .Where(conversation => conversation.Participants.Any(participant => participant.UserId == userId))
            .Include(conversation => conversation.Participants)
            .ThenInclude(participant => participant.User)
            .Include(conversation => conversation.Messages
                .Where(message => !message.IsDeleted)
                .OrderByDescending(message => message.SentAtUtc)
                .Take(1))
            .ThenInclude(message => message.Sender)
            .OrderByDescending(conversation => conversation.LastMessageAtUtc ?? conversation.CreatedAtUtc)
            .ToListAsync(ct);

    public async Task<bool> IsParticipantAsync(Guid conversationId, Guid userId, CancellationToken ct = default)
        => await Accessor.Set<ConversationParticipant>()
            .AnyAsync(participant =>
                participant.ConversationId == conversationId &&
                participant.UserId == userId, ct);

    public async Task<ConversationParticipant?> GetParticipantAsync(
        Guid conversationId,
        Guid userId,
        CancellationToken ct = default)
        => await Accessor.Set<ConversationParticipant>()
            .FirstOrDefaultAsync(participant =>
                participant.ConversationId == conversationId &&
                participant.UserId == userId, ct);
}
