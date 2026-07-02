using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Chat.App.Wpf.Messages;

public sealed class AuthenticationStateChangedMessage(AuthenticationState value)
    : ValueChangedMessage<AuthenticationState>(value);

public sealed record AuthenticationState(bool IsAuthenticated, string Username);
