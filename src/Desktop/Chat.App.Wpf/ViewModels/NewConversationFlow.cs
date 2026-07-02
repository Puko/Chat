using Chat.App.Contract.Chat;
using Chat.App.Wpf.Navigation;
using Chat.App.Wpf.Services;

namespace Chat.App.Wpf.ViewModels;

public static class NewConversationFlow
{
    public static async Task RunAsync(IDialogService dialogService, INavigationService navigation)
    {
        var conversation = await dialogService.ShowAsync<DialogViewModels.NewConversationDialogViewModel, ConversationDto>();
        if (conversation is null)
            return;

        await navigation.Navigate<ConversationViewModel>(vm =>
        {
            vm.ConversationId = conversation.Id;
            vm.Title = GetConversationTitle(conversation);
        });
    }

    private static string GetConversationTitle(ConversationDto conversation)
    {
        var other = conversation.Participants.FirstOrDefault();
        return other?.Username ?? conversation.Id.ToString();
    }
}
