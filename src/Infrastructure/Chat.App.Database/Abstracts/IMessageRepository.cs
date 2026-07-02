using Chat.App.Database.Abstraction;
using Chat.App.Database.Entities;

namespace Chat.App.Database.Abstracts;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<(IReadOnlyList<Message> Items, int TotalCount)> GetForConversationAsync(
        Guid conversationId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<int> CountUnreadAsync(Guid conversationId, Guid userId, DateTime? lastReadAtUtc, CancellationToken ct = default);
}
