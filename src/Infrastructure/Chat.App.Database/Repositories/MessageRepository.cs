using Chat.App.Database.Abstracts;
using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstraction.Abstracts;
using Chat.App.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.App.Database.Repositories;

public sealed class MessageRepository(IDbContextAccessor dbContextAccessor)
    : GenericRepository<Message>(dbContextAccessor), IMessageRepository
{
    public async Task<(IReadOnlyList<Message> Items, int TotalCount)> GetForConversationAsync(
        Guid conversationId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = Accessor.Set<Message>()
            .AsNoTracking()
            .Include(message => message.Sender)
            .Where(message => message.ConversationId == conversationId && !message.IsDeleted);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(message => message.SentAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        items.Reverse();

        return (items, totalCount);
    }

    public async Task<int> CountUnreadAsync(
        Guid conversationId,
        Guid userId,
        DateTime? lastReadAtUtc,
        CancellationToken ct = default)
    {
        var query = Accessor.Set<Message>()
            .Where(message =>
                message.ConversationId == conversationId &&
                !message.IsDeleted &&
                message.SenderId != userId);

        if (lastReadAtUtc is not null)
            query = query.Where(message => message.SentAtUtc > lastReadAtUtc);

        return await query.CountAsync(ct);
    }
}
