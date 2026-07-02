namespace Chat.App.Wpf.Navigation;

public interface IDialogService
{
    void Register<TViewModel, TView>()
        where TViewModel : class, IDialogAware
        where TView : System.Windows.FrameworkElement, new();

    Task ShowAsync<TViewModel>(Action<TViewModel>? configure = null, CancellationToken ct = default)
        where TViewModel : class, IDialogAware;

    Task<TResult?> ShowAsync<TViewModel, TResult>(Action<TViewModel>? configure = null, CancellationToken ct = default)
        where TViewModel : class, IDialogAware<TResult>;
}
