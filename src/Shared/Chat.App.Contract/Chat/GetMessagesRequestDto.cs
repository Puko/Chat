namespace Chat.App.Contract.Chat;

public sealed class GetMessagesRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
