using Chat.App.Contract.Chat;
using Chat.App.Wpf.Localization;

namespace Chat.App.Wpf.Models;

public sealed class ChatMessageModel
{
    public Guid Id { get; init; }

    public string SenderDisplayName { get; init; } = string.Empty;

    public string SentAtText { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public bool IsOwnMessage { get; init; }

    public string Initial { get; init; } = string.Empty;

    public string SenderLine => $"{SenderDisplayName} - {SentAtText}";

    public static ChatMessageModel FromDto(MessageDto dto, Guid currentUserId)
    {
        var isOwnMessage = dto.SenderId == currentUserId;
        var senderDisplayName = isOwnMessage
            ? StringLocalizer.Instance["Conversation_You"]
            : dto.SenderUsername;

        return new ChatMessageModel
        {
            Id = dto.Id,
            SenderDisplayName = senderDisplayName,
            SentAtText = dto.SentAtUtc.ToLocalTime().ToString("HH:mm"),
            Content = dto.Content,
            IsOwnMessage = isOwnMessage,
            Initial = GetInitial(senderDisplayName)
        };
    }

    private static string GetInitial(string value)
    {
        var first = value.FirstOrDefault(char.IsLetterOrDigit);
        return first == 0 ? "#" : char.ToUpperInvariant(first).ToString();
    }
}
