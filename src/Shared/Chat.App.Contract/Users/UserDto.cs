namespace Chat.App.Contract.Users;

public sealed record UserDto(Guid Id, string Username, UserRole Role, DateTime CreatedAtUtc);
