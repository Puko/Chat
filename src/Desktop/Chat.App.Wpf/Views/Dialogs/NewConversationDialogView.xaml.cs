using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Chat.App.Wpf.Models;
using Chat.App.Wpf.ViewModels;
using NewConversationDialogViewModel = Chat.App.Wpf.ViewModels.DialogViewModels.NewConversationDialogViewModel;

namespace Chat.App.Wpf.Views.Dialogs;

public partial class NewConversationDialogView : UserControl
{
    public NewConversationDialogView()
    {
        InitializeComponent();
    }

    private void SearchResults_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not NewConversationDialogViewModel viewModel)
            return;

        var element = e.OriginalSource as DependencyObject;
        while (element is not null and not ListBoxItem)
            element = VisualTreeHelper.GetParent(element);

        if (element is not ListBoxItem { Content: UserLookupModel user })
            return;

        if (viewModel.SelectUserCommand.CanExecute(user))
            viewModel.SelectUserCommand.Execute(user);
    }
}
