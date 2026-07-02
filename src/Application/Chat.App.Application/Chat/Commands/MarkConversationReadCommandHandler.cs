using Chat.App.Application.Common;
using Chat.App.Application.Errors;
using Chat.App.Application.Exceptions;
using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstracts;
using MediatR;

namespace Chat.App.Application.Chat.Commands;

public sealed class MarkConversationReadCommandHandler(
    ICurrentUser currentUser,
    IConversationRepository conversationRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<MarkConversationReadCommand>
{
    public async Task Handle(MarkConversationReadCommand request, CancellationToken ct)
    {
        var userId = currentUser.RequireUserId();

        var participant = await conversationRepository.GetParticipantAsync(
            request.ConversationId,
            userId,
            ct);

        if (participant is null)
            throw new ChatAppException(ChatErrorCodes.NotParticipant, "You are not a participant of this conversation.");

        participant.LastReadAtUtc = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(ct);
    }
}
