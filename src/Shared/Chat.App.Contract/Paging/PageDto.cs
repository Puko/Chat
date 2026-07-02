namespace Chat.App.Contract.Paging;

public sealed class PageDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
