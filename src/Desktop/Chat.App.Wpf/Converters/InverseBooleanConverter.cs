using System.Globalization;
using System.Windows.Data;

namespace Chat.App.Wpf.Converters;

public sealed class InverseBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not bool flag || !flag;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool flag && !flag;
}
