namespace Chat.App.Contract.Auth;

public sealed record RegisterUserResponseDto(Guid Id, string Username, DateTime CreatedAtUtc);
