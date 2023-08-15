using DynamicAero2;

namespace ThLaunchSite
{
    internal class ApplicationTheme
    {
        public static void SetApplicationTheme(string themeName)
        {
            Theme theme = Application.Current.Resources.MergedDictionaries[0] as Theme;
            if (theme != null)
            {
                if (themeName == "Light")
                {
                    theme.Color = ThemeColor.Light;
                }
                else if (themeName == "Dark")
                {
                    theme.Color = ThemeColor.Dark;
                }
                else if (themeName == "Black")
                {
                    theme.Color = ThemeColor.Black;
                }
                else
                {
                    theme.Color = ThemeColor.NormalColor;
                }
            }
        }
    }
}
