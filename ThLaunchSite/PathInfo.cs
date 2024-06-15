namespace ThLaunchSite
{
    internal class PathInfo
    {
        public static string AppPath => typeof(App).Assembly.Location;

        public static string? AppLocation => Path.GetDirectoryName(AppPath);

        public static string? DynamicAero2DllPath => $"{AppLocation}\\DynamicAero2.dll";

        public static string? NAudioCoreDllPath => $"{AppLocation}\\NAudio.Core.dll";

        public static string? NAudioWasapiDllPath => $"{AppLocation}\\NAudio.Wasapi.dll";

        public static string? SettingsDirectory => $"{AppLocation}\\settings";

        public static string? PluginDirectory => $"{AppLocation}\\plugins";

        public static string? ScoreBackupDirectory => $"{AppLocation}\\backup";

        public static string? GameCaptureDirectory => $"{AppLocation}\\GameCapture";

        public static string GamePathSettingsFile
        {
            get
            {
                string gamePathSettingsFile = $"{SettingsDirectory}\\ThLaunchSite.GamePath.xml";
                return !string.IsNullOrEmpty(SettingsDirectory) ? gamePathSettingsFile : string.Empty;
            }
        }

        public static string GameEditionSettingsFile
        {
            get
            {
                string gameEditionSettingsFile = $"{SettingsDirectory}\\ThLaunchSite.GameEdition.xml";
                return !string.IsNullOrEmpty(SettingsDirectory) ? gameEditionSettingsFile : string.Empty;
            }
        }

        public static string ApplicationSettingsFile
        {
            get
            {
                string applicationSettingsFile = $"{SettingsDirectory}\\ThLaunchSite.ApplicationSettings.xml";
                return !string.IsNullOrEmpty(SettingsDirectory) ? applicationSettingsFile : string.Empty;
            }
        }

        public static string ExternalToolsConfig
        {
            get
            {
                string externalToolsConfigFile = $"{SettingsDirectory}\\ThLaunchSite.ExternalTools.xml";
                return !string.IsNullOrEmpty(SettingsDirectory) ? externalToolsConfigFile : string.Empty;
            }
        }

        public static string ReadmeFile => $"{AppLocation}\\Doc\\ReadMe.txt";

        public static string LicenseFile => $"{AppLocation}\\Doc\\License.txt";

        public static string GamePlayLogFile => $"{AppLocation}\\GamePlayLog.xml";

        public static string ShanghaiAliceAppData => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ShanghaiAlice";
    }
}
