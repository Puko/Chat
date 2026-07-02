using System.Windows.Controls;
using System.Windows.Input;
using Chat.App.Wpf.Models;
using Chat.App.Wpf.ViewModels;

namespace Chat.App.Wpf.Views;

public partial class ConversationsListView : UserControl
{
    public ConversationsListView()
    {
        InitializeComponent();
    }

    private async void ListBoxItem_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not ConversationsListViewModel viewModel)
            return;

        if (sender is not ListBoxItem { Content: ConversationItemModel item })
            return;

        if (viewModel.OpenConversationCommand.CanExecute(item))
            await viewModel.OpenConversationCommand.ExecuteAsync(item);
    }
}
