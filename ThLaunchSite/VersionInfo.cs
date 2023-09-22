namespace ThLaunchSite
{
    internal class VersionInfo
    {
        public static readonly string _appPath = PathInfo.AppPath;

        public static string? AppName => FileVersionInfo.GetVersionInfo(_appPath).ProductName;

        public static string? AppVersion => FileVersionInfo.GetVersionInfo(_appPath).ProductVersion + " Beta Preview";

        public static string? Developer => FileVersionInfo.GetVersionInfo(_appPath).CompanyName;

        public static string? Copyright => FileVersionInfo.GetVersionInfo(_appPath).LegalCopyright;

        public static string DotNetVersion => $".NET {Environment.Version}";
    }
}
