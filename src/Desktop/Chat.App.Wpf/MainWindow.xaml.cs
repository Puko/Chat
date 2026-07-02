using System.Windows;
using Chat.App.Wpf.ViewModels;

namespace Chat.App.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel shell)
    {
        InitializeComponent();
        DataContext = shell;
    }

    public void SetCurrentView(FrameworkElement view)
        => CurrentViewHost.Content = view;
}
