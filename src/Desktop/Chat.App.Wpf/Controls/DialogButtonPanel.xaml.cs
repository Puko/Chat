using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace Chat.App.Wpf.Controls;

public partial class DialogButtonPanel : UserControl
{
    public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(
        nameof(CancelCommand), typeof(ICommand), typeof(DialogButtonPanel));

    public static readonly DependencyProperty ConfirmCommandProperty = DependencyProperty.Register(
        nameof(ConfirmCommand), typeof(ICommand), typeof(DialogButtonPanel));

    public static readonly DependencyProperty CancelTextProperty = DependencyProperty.Register(
        nameof(CancelText), typeof(string), typeof(DialogButtonPanel));

    public static readonly DependencyProperty ConfirmTextProperty = DependencyProperty.Register(
        nameof(ConfirmText), typeof(string), typeof(DialogButtonPanel));

    public static readonly DependencyProperty ConfirmIconProperty = DependencyProperty.Register(
        nameof(ConfirmIcon), typeof(PackIconKind), typeof(DialogButtonPanel), new PropertyMetadata(PackIconKind.Check));

    public static readonly DependencyProperty IsConfirmDefaultProperty = DependencyProperty.Register(
        nameof(IsConfirmDefault), typeof(bool), typeof(DialogButtonPanel), new PropertyMetadata(true));

    public DialogButtonPanel()
    {
        InitializeComponent();
    }

    public ICommand? CancelCommand
    {
        get => (ICommand?)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    public ICommand? ConfirmCommand
    {
        get => (ICommand?)GetValue(ConfirmCommandProperty);
        set => SetValue(ConfirmCommandProperty, value);
    }

    public string? CancelText
    {
        get => (string?)GetValue(CancelTextProperty);
        set => SetValue(CancelTextProperty, value);
    }

    public string? ConfirmText
    {
        get => (string?)GetValue(ConfirmTextProperty);
        set => SetValue(ConfirmTextProperty, value);
    }

    public PackIconKind ConfirmIcon
    {
        get => (PackIconKind)GetValue(ConfirmIconProperty);
        set => SetValue(ConfirmIconProperty, value);
    }

    public bool IsConfirmDefault
    {
        get => (bool)GetValue(IsConfirmDefaultProperty);
        set => SetValue(IsConfirmDefaultProperty, value);
    }
}
