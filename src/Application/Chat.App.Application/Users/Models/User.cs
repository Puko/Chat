namespace Chat.App.Application.Users.Models;

public sealed class User
{
    public Guid Id { get; init; }
    public required string Username { get; init; }
    public UserRole Role { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
