using Chat.App.Wpf.Navigation;
using Chat.App.Wpf.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chat.App.Wpf.ViewModels;

public abstract partial class BaseViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    public virtual Task OnNavigatedToAsync() => Task.CompletedTask;

    public virtual Task OnNavigatedFromAsync() => Task.CompletedTask;

    protected bool TryHandle<T>(ServiceResult<T> result, out T value)
    {
        if (result.IsSuccess)
        {
            ErrorMessage = null;
            value = result.Value!;
            return true;
        }

        ErrorMessage = result.Error;
        value = default!;
        return false;
    }

    protected bool TryHandle(ServiceResult<bool> result)
    {
        ErrorMessage = result.IsSuccess ? null : result.Error;
        return result.IsSuccess;
    }
}
