using Chat.App.Application.Chat.Models;
using MediatR;

namespace Chat.App.Application.Chat.Commands;

public sealed record GetOrCreateDirectConversationCommand(Guid OtherUserId) : IRequest<Conversation>;

public sealed record SendMessageCommand(Guid ConversationId, string Content) : IRequest<Message>;

public sealed record MarkConversationReadCommand(Guid ConversationId) : IRequest;
