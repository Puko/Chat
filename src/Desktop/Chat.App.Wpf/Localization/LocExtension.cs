using System.Windows.Data;
using System.Windows.Markup;

namespace Chat.App.Wpf.Localization;

[MarkupExtensionReturnType(typeof(object))]
public sealed class LocExtension(string key) : MarkupExtension
{
    public string Key { get; set; } = key;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new Binding($"[{Key}]")
        {
            Source = StringLocalizer.Instance,
            Mode = BindingMode.OneWay
        };

        return binding.ProvideValue(serviceProvider);
    }
}
