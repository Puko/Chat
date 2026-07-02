using System.Windows;
using Chat.App.Wpf.Navigation;
using Chat.App.Wpf.Services;
using Chat.App.Wpf.ViewModels;

namespace Chat.App.Wpf;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Bootstrapper.Initialize();

        var mainWindow = Bootstrapper.Container.GetInstance<MainWindow>();
        MainWindow = mainWindow;
        mainWindow.Show();

        await RestoreSessionOrShowLoginAsync();
    }

    private static async Task RestoreSessionOrShowLoginAsync()
    {
        var navigation = Bootstrapper.Container.GetInstance<INavigationService>();
        var auth = Bootstrapper.Container.GetInstance<AuthService>();

        if (await auth.TryRestoreSessionAsync())
        {
            var tokenStorage = Bootstrapper.Container.GetInstance<TokenStorageService>();
            var websocket = Bootstrapper.Container.GetInstance<IWebsocketChatService>();
            var token = await tokenStorage.GetAccessTokenAsync();

            await websocket.ConnectAsync(token!);
            await navigation.Navigate<ConversationsListViewModel>();
            return;
        }

        await navigation.Navigate<LoginViewModel>();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await Bootstrapper.ShutdownAsync();
        base.OnExit(e);
    }
}
