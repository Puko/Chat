using System.Collections.ObjectModel;
using Chat.App.Contract.Chat;
using Chat.App.Wpf.Localization;
using Chat.App.Wpf.Models;
using Chat.App.Wpf.Navigation;
using Chat.App.Wpf.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chat.App.Wpf.ViewModels;

public partial class ConversationsListViewModel(
    AppSession session,
    AuthService auth,
    ConversationsService conversations,
    IWebsocketChatService websocket,
    INavigationService navigation,
    IDialogService dialogService,
    IStringLocalizer localizer)
    : BaseViewModel
{
    public ObservableCollection<ConversationItemModel> Items { get; } = [];

    [ObservableProperty]
    private string _loggedInUserText = "";

    [RelayCommand]
    private async Task OpenConversationAsync(ConversationItemModel? item)
    {
        if (item is null)
            return;

        await navigation.Navigate<ConversationViewModel>(vm =>
        {
            vm.ConversationId = item.Id;
            vm.Title = item.Title;
        });
    }

    [RelayCommand]
    private Task OpenNewConversationAsync()
        => NewConversationFlow.RunAsync(dialogService, navigation);

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await websocket.DisconnectAsync();
        await auth.LogoutAsync();
        await navigation.Navigate<LoginViewModel>();
    }

    public override Task OnNavigatedToAsync()
    {
        UpdateLoggedInUserText();
        websocket.MessageReceived += OnMessageReceived;
        localizer.PropertyChanged += OnLocalizerPropertyChanged;
        return LoadConversationsAsync();
    }

    public override Task OnNavigatedFromAsync()
    {
        websocket.MessageReceived -= OnMessageReceived;
        localizer.PropertyChanged -= OnLocalizerPropertyChanged;
        return Task.CompletedTask;
    }

    private void OnLocalizerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => UpdateLoggedInUserText();

    private void UpdateLoggedInUserText()
        => LoggedInUserText = localizer.Format("Conversations_LoggedInAs", session.Username);

    private void OnMessageReceived(MessageDto message)
        => _ = LoadConversationsAsync();

    private async Task LoadConversationsAsync()
    {
        var result = await conversations.GetConversationsAsync();
        if (!TryHandle(result, out var items))
            return;

        Items.Clear();

        foreach (var item in items.OrderByDescending(c => c.LastMessageAtUtc ?? DateTime.MinValue))
            Items.Add(ConversationItemModel.FromDto(item));
    }
}
