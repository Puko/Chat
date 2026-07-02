using Chat.App.Wpf.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace Chat.App.Wpf.Models;

public sealed class AppSession(IMessenger messenger)
{
    public Guid UserId { get; private set; }

    public string Username { get; private set; } = string.Empty;

    public bool IsAuthenticated => UserId != Guid.Empty;

    public void SetUser(Guid userId, string username)
    {
        UserId = userId;
        Username = username;
        PublishState();
    }

    public void Clear()
    {
        UserId = Guid.Empty;
        Username = "";
        PublishState();
    }

    private void PublishState()
        => messenger.Send(new AuthenticationStateChangedMessage(
            new AuthenticationState(IsAuthenticated, Username)));
}
