namespace Chat.App.Wpf.Localization;

public static class AppCultureExtensions
{
    public static AppCulture ParseCode(string code) =>
        code.Equals("sk", StringComparison.OrdinalIgnoreCase) ? AppCulture.Sk : AppCulture.En;
}