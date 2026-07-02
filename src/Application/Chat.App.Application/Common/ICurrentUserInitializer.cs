namespace Chat.App.Application.Common;

public interface ICurrentUserInitializer
{
    void SetUserId(Guid userId);
}
