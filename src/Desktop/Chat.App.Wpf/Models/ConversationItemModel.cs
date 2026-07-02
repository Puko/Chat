using Chat.App.Contract.Chat;
using Chat.App.Wpf.Localization;

namespace Chat.App.Wpf.Models;

public sealed class ConversationItemModel
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Preview { get; init; } = string.Empty;

    public string Initial { get; init; } = string.Empty;

    public int UnreadCount { get; init; }

    public bool HasUnread => UnreadCount > 0;

    public string UnreadBadgeText => UnreadCount > 99 ? "99+" : UnreadCount.ToString();

    public string LastActivityText { get; init; } = "";

    public static ConversationItemModel FromDto(ConversationSummaryDto dto)
    {
        var title = dto.OtherParticipantUsername ?? dto.Id.ToString();
        var preview = string.IsNullOrWhiteSpace(dto.LastMessagePreview)
            ? StringLocalizer.Instance["Conversations_EmptyPreview"]
            : dto.LastMessagePreview!;

        return new ConversationItemModel
        {
            Id = dto.Id,
            Title = title,
            Preview = preview,
            Initial = GetInitial(title),
            UnreadCount = dto.UnreadCount,
            LastActivityText = FormatLastActivity(dto.LastMessageAtUtc)
        };
    }

    private static string FormatLastActivity(DateTime? lastMessageAtUtc)
    {
        if (!lastMessageAtUtc.HasValue)
            return StringLocalizer.Instance["Time_New"];

        var local = lastMessageAtUtc.Value.ToLocalTime();
        var today = DateTime.Now.Date;

        if (local.Date == today)
            return local.ToString("HH:mm");

        if (local.Year == today.Year)
            return local.ToString("dd.MM");

        return local.ToString("dd.MM.yy");
    }

    private static string GetInitial(string value)
    {
        var first = value.FirstOrDefault(char.IsLetterOrDigit);
        return first == 0 ? "#" : char.ToUpperInvariant(first).ToString();
    }
}
