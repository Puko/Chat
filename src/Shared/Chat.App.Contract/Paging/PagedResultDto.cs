namespace Chat.App.Contract.Paging;

public sealed class PagedResultDto<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}
