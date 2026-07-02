using Chat.App.Database.Abstraction;
using Chat.App.Database.Entities;

namespace Chat.App.Database.Abstracts;

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation?> FindDirectConversationAsync(Guid userId1, Guid userId2, CancellationToken ct = default);

    Task<Conversation?> GetByIdForUserAsync(Guid conversationId, Guid userId, CancellationToken ct = default);

    Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId, CancellationToken ct = default);

    Task<bool> IsParticipantAsync(Guid conversationId, Guid userId, CancellationToken ct = default);

    Task<ConversationParticipant?> GetParticipantAsync(Guid conversationId, Guid userId, CancellationToken ct = default);
}
