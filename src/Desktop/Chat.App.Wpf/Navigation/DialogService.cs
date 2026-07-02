using System.Windows;
using MaterialDesignThemes.Wpf;
using SimpleInjector;

namespace Chat.App.Wpf.Navigation;

public sealed class DialogService(Container container) : IDialogService
{
    public const string RootIdentifier = "RootDialog";

    private readonly Dictionary<Type, Type> _registrations = new();

    public void Register<TViewModel, TView>()
        where TViewModel : class, IDialogAware
        where TView : FrameworkElement, new()
    {
        _registrations[typeof(TViewModel)] = typeof(TView);
        container.Register<TViewModel>();
        container.Register<TView>();
    }

    public Task ShowAsync<TViewModel>(Action<TViewModel>? configure = null, CancellationToken ct = default)
        where TViewModel : class, IDialogAware
        => Application.Current.Dispatcher.CheckAccess()
            ? ShowCoreAsync(configure, ct)
            : Application.Current.Dispatcher.InvokeAsync(
                () => ShowCoreAsync(configure, ct)).Task.Unwrap();

    public Task<TResult?> ShowAsync<TViewModel, TResult>(
        Action<TViewModel>? configure = null,
        CancellationToken ct = default)
        where TViewModel : class, IDialogAware<TResult>
        => Application.Current.Dispatcher.CheckAccess()
            ? ShowCoreAsync<TViewModel, TResult>(configure, ct)
            : Application.Current.Dispatcher.InvokeAsync(
                () => ShowCoreAsync<TViewModel, TResult>(configure, ct)).Task.Unwrap();

    private async Task ShowCoreAsync<TViewModel>(
        Action<TViewModel>? configure,
        CancellationToken ct)
        where TViewModel : class, IDialogAware
    {
        await ShowDialogAsync(configure, ct);
    }

    private async Task<TResult?> ShowCoreAsync<TViewModel, TResult>(
        Action<TViewModel>? configure,
        CancellationToken ct)
        where TViewModel : class, IDialogAware<TResult>
    {
        var dialogResult = await ShowDialogAsync(configure, ct);
        return dialogResult is TResult typedResult ? typedResult : default;
    }

    private async Task<object?> ShowDialogAsync<TViewModel>(
        Action<TViewModel>? configure,
        CancellationToken ct)
        where TViewModel : class, IDialogAware
    {
        if (!_registrations.TryGetValue(typeof(TViewModel), out var viewType))
        {
            throw new InvalidOperationException(
                $"View for dialog ViewModel '{typeof(TViewModel).Name}' is not registered.");
        }

        var viewModel = container.GetInstance<TViewModel>();
        configure?.Invoke(viewModel);

        var view = (FrameworkElement)container.GetInstance(viewType);
        viewModel.Context = new DialogContext(RootIdentifier);
        view.DataContext = viewModel;

        await viewModel.OnOpenedAsync();

        var dialogResult = await DialogHost.Show(view, RootIdentifier);

        await viewModel.OnClosingAsync();

        return dialogResult;
    }

    private sealed class DialogContext(object identifier) : IDialogContext
    {
        public void Close(object? result = null)
            => DialogHost.Close(identifier, result);

        public void Cancel()
            => Close();
    }
}
