using Chat.App.Application.Chat.Extensions;
using Chat.App.Application.Chat.Models;
using Chat.App.Application.Common;
using Chat.App.Application.Errors;
using Chat.App.Application.Exceptions;
using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstracts;
using MediatR;
using DbConversation = Chat.App.Database.Entities.Conversation;
using DbConversationParticipant = Chat.App.Database.Entities.ConversationParticipant;
using DbConversationType = Chat.App.Database.Entities.ConversationType;

namespace Chat.App.Application.Chat.Commands;

public sealed class GetOrCreateDirectConversationCommandHandler(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IConversationRepository conversationRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GetOrCreateDirectConversationCommand, Conversation>
{
    public async Task<Conversation> Handle(GetOrCreateDirectConversationCommand request, CancellationToken ct)
    {
        var userId = currentUser.RequireUserId();

        if (userId == request.OtherUserId)
            throw new ChatAppException(ChatErrorCodes.CannotChatWithSelf, "You cannot start a chat with yourself.");

        var otherUser = await userRepository.GetByIdAsync(ct, request.OtherUserId);
        if (otherUser is null)
            throw new ChatAppException(ChatErrorCodes.UserNotFound, "User not found.");

        var existing = await conversationRepository.FindDirectConversationAsync(
            userId,
            request.OtherUserId,
            ct);

        if (existing is not null)
            return existing.ToConversation();

        var now = DateTime.UtcNow;

        var conversation = new DbConversation
        {
            Id = Guid.NewGuid(),
            Type = DbConversationType.Direct,
            CreatedAtUtc = now,
            Participants =
            [
                new DbConversationParticipant
                {
                    UserId = userId,
                    JoinedAtUtc = now
                },
                new DbConversationParticipant
                {
                    UserId = request.OtherUserId,
                    JoinedAtUtc = now
                }
            ]
        };

        await conversationRepository.AddAsync(conversation, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var created = await conversationRepository.GetByIdForUserAsync(conversation.Id, userId, ct)
            ?? throw new ChatAppException(ChatErrorCodes.ConversationNotFound, "Conversation not found.");

        return created.ToConversation();
    }
}
