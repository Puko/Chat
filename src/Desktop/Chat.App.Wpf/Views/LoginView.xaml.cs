using System.Windows.Controls;
using Chat.App.Wpf.ViewModels;

namespace Chat.App.Wpf.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void PasswordInput_OnPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel)
            viewModel.Password = PasswordInput.Password;
    }
}
