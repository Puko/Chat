namespace Chat.App.Wpf.Navigation;

public interface INavigationAware
{
    Task OnNavigatedToAsync();

    Task OnNavigatedFromAsync();
}
