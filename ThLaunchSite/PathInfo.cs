namespace ThLaunchSite
{
    internal class PathInfo
    {
        public static string AppPath => typeof(App).Assembly.Location;

        public static string? AppLocation => Path.GetDirectoryName(AppPath);

        public static string? SettingsDirectory => $"{AppLocation}\\settings";

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

        public static string Readme => $"{AppLocation}\\Doc\\ReadMe.txt";

        public static string LicenseFile => $"{AppLocation}\\Doc\\License.txt";

        public static string GamePlayLogFile => $"{AppLocation}\\GamePlayLog.xml";
    }
}
