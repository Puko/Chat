using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Chat.App.Contract.Chat;
using Chat.App.Wpf.Localization;
using Chat.App.Wpf.Models;
using Chat.App.Wpf.Navigation;
using Chat.App.Wpf.Services;
using Chat.App.Wpf.Services.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chat.App.Wpf.ViewModels;

public partial class ConversationViewModel(
    AppSession session,
    ConversationsService conversations,
    IWebsocketChatService websocket,
    INavigationService navigation,
    HttpErrorHandler errorHandler,
    IStringLocalizer localizer)
    : BaseViewModel
{
    public Guid ConversationId { get; set; }

    public ObservableCollection<ChatMessageModel> Messages { get; } = [];

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _messageText = string.Empty;

    [ObservableProperty]
    private bool _isSending;

    [ObservableProperty]
    private string _messageCountText = string.Empty;

    public bool CanSend => !IsSending && !string.IsNullOrWhiteSpace(MessageText) && websocket.IsConnected;

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task SendAsync()
    {
        var content = MessageText.Trim();
        if (string.IsNullOrWhiteSpace(content))
            return;

        IsSending = true;
        SendCommand.NotifyCanExecuteChanged();
        ErrorMessage = null;

        try
        {
            await websocket.SendMessageAsync(ConversationId, content);
            MessageText = "";
        }
        catch (Exception ex)
        {
            ErrorMessage = errorHandler.GetMessage(ex);
        }
        finally
        {
            IsSending = false;
            SendCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private Task GoBackAsync()
        => navigation.Navigate<ConversationsListViewModel>();

    public override async Task OnNavigatedToAsync()
    {
        websocket.MessageReceived += OnMessageReceived;
        websocket.ErrorReceived += OnWebSocketError;
        Messages.CollectionChanged += OnMessagesCollectionChanged;
        localizer.PropertyChanged += OnLocalizerPropertyChanged;
        UpdateMessageCountText();
        await LoadMessagesAsync();
    }

    public override Task OnNavigatedFromAsync()
    {
        websocket.MessageReceived -= OnMessageReceived;
        websocket.ErrorReceived -= OnWebSocketError;
        Messages.CollectionChanged -= OnMessagesCollectionChanged;
        localizer.PropertyChanged -= OnLocalizerPropertyChanged;
        return Task.CompletedTask;
    }

    private void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => UpdateMessageCountText();

    private void OnLocalizerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        => UpdateMessageCountText();

    private void UpdateMessageCountText()
        => MessageCountText = localizer.Format("Conversation_MessageCount", Messages.Count);

    partial void OnMessageTextChanged(string value)
        => SendCommand.NotifyCanExecuteChanged();

    partial void OnIsSendingChanged(bool value)
        => SendCommand.NotifyCanExecuteChanged();

    private void OnWebSocketError(string error)
        => ErrorMessage = error;

    private void OnMessageReceived(MessageDto message)
    {
        if (message.ConversationId != ConversationId)
            return;

        if (Messages.Any(m => m.Id == message.Id))
            return;

        Messages.Add(ChatMessageModel.FromDto(message, session.UserId));
    }

    private async Task LoadMessagesAsync()
    {
        var result = await conversations.GetMessagesAsync(ConversationId);
        if (!TryHandle(result, out var page))
            return;

        Messages.Clear();

        foreach (var message in page.Items.OrderBy(m => m.SentAtUtc))
            Messages.Add(ChatMessageModel.FromDto(message, session.UserId));
    }
}
