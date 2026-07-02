using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chat.App.Wpf.ViewModels.DialogViewModels;

/// <summary>
/// Generic yes/no confirmation dialog. Caller sets <see cref="Title"/>, <see cref="Message"/>,
/// <see cref="ConfirmText"/> and <see cref="CancelText"/> via the configure callback passed to
/// <c>IDialogService.ShowAsync</c>. Result is <c>true</c> when confirmed, <c>false</c> otherwise
/// (including when the dialog is dismissed).
/// </summary>
public partial class ConfirmationDialogViewModel : DialogViewModelBase<bool>
{
    [ObservableProperty]
    private string _title = "";

    [ObservableProperty]
    private string _message = "";

    [ObservableProperty]
    private string _confirmText = "";

    [ObservableProperty]
    private string _cancelText = "";

    [RelayCommand]
    private void Confirm()
        => Close(true);

    [RelayCommand]
    private void CancelDialog()
        => Cancel();
}
