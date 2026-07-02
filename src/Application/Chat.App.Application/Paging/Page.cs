namespace Chat.App.Application.Paging;

public sealed class Page
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
