using System.Windows;
using SimpleInjector;

namespace Chat.App.Wpf.Navigation;

public sealed class NavigationService(Container container)
    : INavigationService
{
    private readonly Dictionary<Type, Type> _registrations = new();
    private INavigationAware? _current;

    public void Register<TViewModel, TView>()
        where TViewModel : class, INavigationAware
        where TView : FrameworkElement, new()
    {
        _registrations[typeof(TViewModel)] = typeof(TView);
        container.Register<TViewModel>();
        container.Register<TView>();
    }

    public async Task Navigate<TViewModel>(Action<TViewModel>? configure = null)
        where TViewModel : class, INavigationAware
    {
        if (!_registrations.TryGetValue(typeof(TViewModel), out var viewType))
        {
            throw new InvalidOperationException(
                $"View for ViewModel '{typeof(TViewModel).Name}' is not registered");
        }

        if (_current is not null)
            await _current.OnNavigatedFromAsync();

        var viewModel = container.GetInstance<TViewModel>();
        configure?.Invoke(viewModel);

        var view = (FrameworkElement)container.GetInstance(viewType);
        view.DataContext = viewModel;

        _current = viewModel;

        if (Application.Current.MainWindow is not MainWindow window)
            throw new InvalidOperationException("MainWindow is not available.");

        window.SetCurrentView(view);

        await viewModel.OnNavigatedToAsync();
    }
}
