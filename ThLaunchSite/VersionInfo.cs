namespace ThLaunchSite
{
    internal class VersionInfo
    {
        private static readonly string _appPath = typeof(App).Assembly.Location;

        public static string? AppName => FileVersionInfo.GetVersionInfo(_appPath).ProductName;

        public static string? AppVersion => FileVersionInfo.GetVersionInfo(_appPath).ProductVersion;

        public static string? Developer => FileVersionInfo.GetVersionInfo(_appPath).CompanyName;
    }
}
