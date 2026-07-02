namespace Chat.App.Application.Common;

public interface ICurrentUser
{
    Guid? UserId { get; }
    bool IsInRole(string role);
}
