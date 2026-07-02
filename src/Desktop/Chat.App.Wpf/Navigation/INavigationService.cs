using System.Windows;

namespace Chat.App.Wpf.Navigation;

public interface INavigationService
{
    void Register<TViewModel, TView>()
        where TViewModel : class, INavigationAware
        where TView : FrameworkElement, new();

    Task Navigate<TViewModel>(Action<TViewModel>? configure = null)
        where TViewModel : class, INavigationAware;
}
