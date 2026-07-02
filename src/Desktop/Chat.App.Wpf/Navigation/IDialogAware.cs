namespace Chat.App.Wpf.Navigation;

public interface IDialogAware
{
    IDialogContext Context { get; set; }

    Task OnOpenedAsync();

    Task OnClosingAsync();
}

public interface IDialogAware<TResult> : IDialogAware;
