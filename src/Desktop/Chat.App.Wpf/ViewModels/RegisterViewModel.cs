using Chat.App.Contract.Auth;
using Chat.App.Wpf.Localization;
using Chat.App.Wpf.Navigation;
using Chat.App.Wpf.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chat.App.Wpf.ViewModels;

public partial class RegisterViewModel(
    AuthService auth,
    TokenStorageService tokenStorage,
    IWebsocketChatService websocket,
    INavigationService navigation,
    IStringLocalizer localizer)
    : BaseViewModel
{
    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy)
            return;

        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = localizer["Errors_RequiredFields"];
            return;
        }

        IsBusy = true;
        try
        {
            var result = await auth.RegisterAsync(new RegisterUser
            {
                Username = Username.Trim(),
                Password = Password
            });

            if (!TryHandle(result, out _))
                return;

            var token = await tokenStorage.GetAccessTokenAsync();
            await websocket.ConnectAsync(token!);

            Password = "";
            await navigation.Navigate<ConversationsListViewModel>();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task GoToLoginAsync()
        => navigation.Navigate<LoginViewModel>();
}
