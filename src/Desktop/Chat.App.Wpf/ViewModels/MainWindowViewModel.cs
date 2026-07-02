using Chat.App.Wpf.Localization;
using Chat.App.Wpf.Messages;
using Chat.App.Wpf.Models;
using Chat.App.Wpf.Navigation;
using Chat.App.Wpf.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Chat.App.Wpf.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IRecipient<AuthenticationStateChangedMessage>
{
    private readonly AuthService _auth;
    private readonly IWebsocketChatService _websocket;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogService;
    private readonly IStringLocalizer _localizer;

    [ObservableProperty]
    private bool _showUserMenu;

    [ObservableProperty]
    private string _loggedInUsername = string.Empty;

    [ObservableProperty]
    private string _loggedInUserInitial = string.Empty;

    public MainWindowViewModel(
        AppSession session,
        AuthService auth,
        IWebsocketChatService websocket,
        INavigationService navigation,
        IDialogService dialogService,
        IStringLocalizer localizer,
        IMessenger messenger)
    {
        _auth = auth;
        _websocket = websocket;
        _navigation = navigation;
        _dialogService = dialogService;
        _localizer = localizer;

        messenger.Register(this);
        ApplyAuthenticationState(new AuthenticationState(session.IsAuthenticated, session.Username));
    }

    [RelayCommand]
    private void SetLanguage(string cultureCode)
        => _localizer.SetCulture(AppCultureExtensions.ParseCode(cultureCode));

    [RelayCommand]
    private Task OpenNewConversationAsync()
        => NewConversationFlow.RunAsync(_dialogService, _navigation);

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirmed = await _dialogService.ShowAsync<DialogViewModels.ConfirmationDialogViewModel, bool>(vm =>
        {
            vm.Title = _localizer["Shell_LogoutConfirmTitle"];
            vm.Message = _localizer["Shell_LogoutConfirmMessage"];
            vm.ConfirmText = _localizer["Shell_Logout"];
            vm.CancelText = _localizer["Common_Cancel"];
        });

        if (!confirmed)
            return;

        await _websocket.DisconnectAsync();
        await _auth.LogoutAsync();
        await _navigation.Navigate<LoginViewModel>();
    }

    public void Receive(AuthenticationStateChangedMessage message)
        => ApplyAuthenticationState(message.Value);

    private void ApplyAuthenticationState(AuthenticationState state)
    {
        ShowUserMenu = state.IsAuthenticated;
        LoggedInUsername = state.Username;
        LoggedInUserInitial = string.IsNullOrWhiteSpace(state.Username)
            ? string.Empty
            : state.Username[..1].ToUpperInvariant();
    }
}
