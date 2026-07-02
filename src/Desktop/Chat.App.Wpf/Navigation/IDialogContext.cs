namespace Chat.App.Wpf.Navigation;

public interface IDialogContext
{
    void Close(object? result = null);

    void Cancel();
}
