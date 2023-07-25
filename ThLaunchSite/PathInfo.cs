namespace ThLaunchSite
{
    internal class PathInfo
    {
        public static string AppPath => typeof(App).Assembly.Location;

        public static string? AppLocation => Path.GetDirectoryName(AppPath);

        public static string UsersDirectory => $"{AppLocation}\\Users";

        public static string UserIndex => $"{UsersDirectory}\\index.xml";

        public static string UserSelectionConfig => $"{AppLocation}\\UserSelectionConfig.xml";

        public static string SettingsDirectory
        {
            get
            {
                string currentUserDirectory = $"{UsersDirectory}\\{User.CurrentUserDirectoryName}";
                return !string.IsNullOrEmpty(User.CurrentUserDirectoryName) ? currentUserDirectory : string.Empty;
            }
        }

        public static string GamePathSettingsFile
        {
            get
            {
                string gamePathSettingsFile = $"{SettingsDirectory}\\ThLaunchSite.GamePath.xml";
                return !string.IsNullOrEmpty(SettingsDirectory) ? gamePathSettingsFile : string.Empty;
            }
        }

        public static string MainWindowSettingsFile
        {
            get
            {
                string gamePathSettingsFile = $"{SettingsDirectory}\\ThLaunchSite.MainWindowSettings.xml";
                return !string.IsNullOrEmpty(SettingsDirectory) ? gamePathSettingsFile : string.Empty;
            }
        }
    }
}
