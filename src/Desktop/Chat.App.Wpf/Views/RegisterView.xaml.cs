using System.Windows.Controls;
using Chat.App.Wpf.ViewModels;

namespace Chat.App.Wpf.Views;

public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
    }

    private void PasswordInput_OnPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel viewModel)
            viewModel.Password = PasswordInput.Password;
    }
}
