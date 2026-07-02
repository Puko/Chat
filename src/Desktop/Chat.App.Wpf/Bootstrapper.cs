using System.Net.Http;
using Chat.App.Wpf.Localization;
using Chat.App.Wpf.Models;
using Chat.App.Wpf.Navigation;
using Chat.App.Wpf.Services;
using Chat.App.Wpf.Services.Http;
using Chat.App.Wpf.Services.Websocket;
using Chat.App.Wpf.ViewModels;
using Chat.App.Wpf.ViewModels.DialogViewModels;
using Chat.App.Wpf.Views;
using Chat.App.Wpf.Views.Dialogs;
using ChatApp.Http;
using CommunityToolkit.Mvvm.Messaging;
using SimpleInjector;

namespace Chat.App.Wpf;

public static class Bootstrapper
{
    public static Container Container { get; private set; } = null!;

    public static void Initialize()
    {
        Container = new Container();
        Container.Options.ResolveUnregisteredConcreteTypes = false;

        RegisterServices(Container);
        var navigation = new NavigationService(Container);
        var dialogService = new DialogService(Container);
        Container.RegisterInstance<INavigationService>(navigation);
        Container.RegisterInstance<IDialogService>(dialogService);
        Container.RegisterSingleton<MainWindowViewModel>();
        navigation.Register<LoginViewModel, LoginView>();
        navigation.Register<RegisterViewModel, RegisterView>();
        navigation.Register<ConversationsListViewModel, ConversationsListView>();
        navigation.Register<ConversationViewModel, ConversationView>();
        dialogService.Register<NewConversationDialogViewModel, NewConversationDialogView>();
        dialogService.Register<ConfirmationDialogViewModel, ConfirmationDialogView>();
        Container.Register<MainWindow>();
        Container.Verify();

        WireUnauthorizedHandler();
    }

    public static async Task ShutdownAsync()
    {
        var realtime = Container.GetInstance<IWebsocketChatService>();
        await realtime.DisconnectAsync();

        if (realtime is IAsyncDisposable disposable)
            await disposable.DisposeAsync();
    }

    private static void RegisterServices(Container container)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(AppSettings.ApiBaseUrl.TrimEnd('/') + "/") };

        container.RegisterInstance(httpClient);
        container.RegisterInstance(JsonOptions.Value);
        container.RegisterSingleton(() =>
            new JsonHttpClient(httpClient, JsonOptions.Value));

        container.RegisterInstance<IStringLocalizer>(StringLocalizer.Instance);
        container.RegisterInstance<IMessenger>(WeakReferenceMessenger.Default);
        container.RegisterSingleton<TokenStorageService>();
        container.RegisterSingleton<AppSession>();
        container.RegisterSingleton<TokenRefreshService>();
        container.RegisterSingleton<HttpErrorHandler>();
        container.RegisterSingleton<AuthService>();
        container.RegisterSingleton<ConversationsService>();
        container.RegisterSingleton<IWebsocketChatService>(() => new WebsocketChatService(AppSettings.ApiBaseUrl));
    }

    private static void WireUnauthorizedHandler()
    {
        var errorHandler = Container.GetInstance<HttpErrorHandler>();
        var navigation = Container.GetInstance<INavigationService>();
        var realtime = Container.GetInstance<IWebsocketChatService>();

        errorHandler.Unauthorized += () =>
            Current.Dispatcher.Invoke(() =>
            {
                _ = realtime.DisconnectAsync();
                _ = navigation.Navigate<LoginViewModel>(vm =>
                    vm.ErrorMessage = StringLocalizer.Instance["Errors_SessionExpired"]);
            });
    }

    private static App Current => (App)System.Windows.Application.Current;
}
