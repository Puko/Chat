using Chat.App.Application.Chat.Extensions;
using Chat.App.Application.Chat.Models;
using Chat.App.Application.Chat.Notifications;
using Chat.App.Application.Common;
using Chat.App.Application.Errors;
using Chat.App.Application.Exceptions;
using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstracts;
using MediatR;
using DbMessage = Chat.App.Database.Entities.Message;

namespace Chat.App.Application.Chat.Commands;

public sealed class SendMessageCommandHandler(
    ICurrentUser currentUser,
    IConversationRepository conversationRepository,
    IMessageRepository messageRepository,
    IUnitOfWork unitOfWork,
    IPublisher publisher)
    : IRequestHandler<SendMessageCommand, Message>
{
    public async Task<Message> Handle(SendMessageCommand request, CancellationToken ct)
    {
        var userId = currentUser.RequireUserId();

        var conversation = await conversationRepository.GetByIdForUserAsync(
            request.ConversationId,
            userId,
            ct);

        if (conversation is null)
            throw new ChatAppException(ChatErrorCodes.NotParticipant, "You are not a participant of this conversation.");

        var now = DateTime.UtcNow;

        var entity = new DbMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId = userId,
            Content = request.Content.Trim(),
            SentAtUtc = now
        };

        await messageRepository.AddAsync(entity, ct);

        conversation.LastMessageAtUtc = now;
        conversationRepository.Update(conversation);

        await unitOfWork.SaveChangesAsync(ct);

        entity.Sender = conversation.Participants
            .First(participant => participant.UserId == userId)
            .User;

        var message = entity.ToMessage();

        var participantUserIds = conversation.Participants
            .Select(participant => participant.UserId)
            .ToList();

        await publisher.Publish(new MessageSentNotification(message, participantUserIds), ct);

        return message;
    }
}
