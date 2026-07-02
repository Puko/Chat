using System.Collections.ObjectModel;
using Chat.App.Contract.Chat;
using Chat.App.Contract.Users;
using Chat.App.Wpf.Localization;
using Chat.App.Wpf.Models;
using Chat.App.Wpf.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chat.App.Wpf.ViewModels.DialogViewModels;

public partial class NewConversationDialogViewModel(
    AppSession session,
    ConversationsService conversations,
    IStringLocalizer localizer)
    : DialogViewModelBase<ConversationDto>
{
    private CancellationTokenSource? _searchCts;
    private bool _suppressUserSearch;

    public ObservableCollection<UserLookupModel> SearchResults { get; } = [];

    [ObservableProperty]
    private string _userSearchText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private UserLookupModel? _selectedUser;

    [ObservableProperty]
    private bool _hasSearchResults;

    [ObservableProperty]
    private bool _isSearchingUsers;

    public string? SelectedUserText
        => SelectedUser is null ? null : localizer.Format("NewConversation_Selected", SelectedUser.Username);

    public override Task OnOpenedAsync()
    {
        localizer.PropertyChanged += OnLocalizerPropertyChanged;
        return Task.CompletedTask;
    }

    public override Task OnClosingAsync()
    {
        localizer.PropertyChanged -= OnLocalizerPropertyChanged;
        return Task.CompletedTask;
    }

    private void OnLocalizerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => OnPropertyChanged(nameof(SelectedUserText));

    partial void OnUserSearchTextChanged(string value)
    {
        if (_suppressUserSearch)
            return;

        SelectedUser = null;
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        _ = SearchUsersDebouncedAsync(value, _searchCts.Token);
    }

    partial void OnSelectedUserChanged(UserLookupModel? value)
        => OnPropertyChanged(nameof(SelectedUserText));

    [RelayCommand]
    private void SelectUser(UserLookupModel? user)
    {
        if (user is null)
            return;

        SelectedUser = user;
        _searchCts?.Cancel();
        _suppressUserSearch = true;
        UserSearchText = user.Username;
        _suppressUserSearch = false;
        SearchResults.Clear();
        HasSearchResults = false;
    }

    [RelayCommand]
    private void CancelDialog()
    {
        _searchCts?.Cancel();
        Cancel();
    }

    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private async Task ConfirmAsync()
    {
        if (SelectedUser is null)
        {
            ErrorMessage = localizer["NewConversation_SelectUserError"];
            return;
        }

        if (SelectedUser.Id == session.UserId)
        {
            ErrorMessage = localizer["NewConversation_CannotSelfError"];
            return;
        }

        IsBusy = true;

        try
        {
            var result = await conversations.CreateDirectAsync(new CreateDirectConversationRequestDto
            {
                OtherUserId = SelectedUser.Id
            });

            if (!TryHandle(result, out var conversation))
                return;

            Close(conversation);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanConfirm() => SelectedUser is not null;

    private async Task SearchUsersDebouncedAsync(string value, CancellationToken ct)
    {
        try
        {
            await Task.Delay(300, ct);

            var search = value.Trim();
            if (search.Length < 2)
            {
                SearchResults.Clear();
                HasSearchResults = false;
                IsSearchingUsers = false;
                return;
            }

            IsSearchingUsers = true;

            var result = await conversations.SearchUsersAsync(new SearchUsersRequestDto
            {
                Search = search,
                PageNumber = 1,
                PageSize = 10
            }, ct);

            if (ct.IsCancellationRequested)
                return;

            SearchResults.Clear();

            if (!TryHandle(result, out var page))
                return;

            foreach (var user in page.Items)
                SearchResults.Add(UserLookupModel.FromDto(user));

            HasSearchResults = SearchResults.Count > 0;
        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            if (!ct.IsCancellationRequested)
                IsSearchingUsers = false;
        }
    }
}
