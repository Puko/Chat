using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Data;

namespace Chat.App.Wpf.Localization;

public sealed class StringLocalizer : IStringLocalizer
{
    public static StringLocalizer Instance { get; } = new();

    private static readonly ResourceManager Resources = new(
        "Chat.App.Wpf.Resources.Strings",
        Assembly.GetExecutingAssembly());

    private static readonly IReadOnlyDictionary<AppCulture, CultureInfo> Cultures = new Dictionary<AppCulture, CultureInfo>
    {
        [AppCulture.Sk] = new("sk"),
        [AppCulture.En] = new("en")
    };

    public event PropertyChangedEventHandler? PropertyChanged;

    public AppCulture CurrentCulture { get; private set; } = AppCulture.Sk;

    public string this[string key]
        => Resources.GetString(key, Cultures[CurrentCulture]) ?? key;

    public string Format(string key, params object[] args)
        => string.Format(this[key], args);

    public void SetCulture(AppCulture culture)
    {
        if (CurrentCulture == culture)
            return;

        CurrentCulture = culture;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Binding.IndexerName));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
    }
}
