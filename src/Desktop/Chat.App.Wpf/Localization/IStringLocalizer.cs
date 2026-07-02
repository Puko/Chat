using System.ComponentModel;

namespace Chat.App.Wpf.Localization;

public interface IStringLocalizer : INotifyPropertyChanged
{
    AppCulture CurrentCulture { get; }

    string this[string key] { get; }

    string Format(string key, params object[] args);

    void SetCulture(AppCulture culture);
}
