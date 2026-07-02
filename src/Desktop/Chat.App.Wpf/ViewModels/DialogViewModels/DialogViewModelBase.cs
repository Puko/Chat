using Chat.App.Wpf.Navigation;

namespace Chat.App.Wpf.ViewModels.DialogViewModels;

public abstract partial class DialogViewModelBase : BaseViewModel, IDialogAware
{
    public IDialogContext Context { get; set; } = null!;

    public virtual Task OnOpenedAsync() => Task.CompletedTask;

    public virtual Task OnClosingAsync() => Task.CompletedTask;

    protected void Cancel()
        => Context.Cancel();
}

public abstract partial class DialogViewModelBase<TResult> : DialogViewModelBase, IDialogAware<TResult>
{
    protected void Close(TResult result)
        => Context.Close(result);
}
