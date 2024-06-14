using DynamicAero2;

namespace ThLaunchSite
{
    internal class ApplicationTheme
    {
        private static string? _themeName;

        public static string? ThemeName
        {
            get
            {
                return _themeName;
            }

            set
            {
                _themeName = value;
                SetApplicationTheme(_themeName);
            }
        }

        private static void SetApplicationTheme(string themeName)
        {
            if (Application.Current.Resources.MergedDictionaries[0] is Theme theme)
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
