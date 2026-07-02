using Chat.App.Application.Chat;
using Chat.App.Application.Chat.Models;
using MediatR;

namespace Chat.App.Application.Chat.Notifications;

public sealed record MessageSentNotification(
    Message Message,
    IReadOnlyList<Guid> ParticipantUserIds) : INotification;
