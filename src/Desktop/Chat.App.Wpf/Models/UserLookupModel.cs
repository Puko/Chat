using Chat.App.Contract.Users;

namespace Chat.App.Wpf.Models;

public sealed class UserLookupModel
{
    public Guid Id { get; init; }

    public string Username { get; init; } = string.Empty;

    public string Initial { get; init; } = string.Empty;

    public static UserLookupModel FromDto(UserLookupDto dto)
        => new()
        {
            Id = dto.Id,
            Username = dto.Username,
            Initial = GetInitial(dto.Username)
        };

    private static string GetInitial(string value)
    {
        var first = value.FirstOrDefault(char.IsLetterOrDigit);
        return first == 0 ? "#" : char.ToUpperInvariant(first).ToString();
    }
}
