namespace Chat.App.Contract.Users;

public sealed class GetUsersRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
}
