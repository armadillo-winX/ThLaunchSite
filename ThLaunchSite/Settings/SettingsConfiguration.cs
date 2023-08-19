using System.Xml.Serialization;

namespace ThLaunchSite.Settings
{
    internal class SettingsConfiguration
    {
        public static GamePathSettings? _gamePathSettings = new();

        public static void SaveGamePathSettings()
        {
            string? gamePathSettingsFile = PathInfo.GamePathSettingsFile;

            _gamePathSettings.Th06 = GamePath.Th06FilePath;
            _gamePathSettings.Th07 = GamePath.Th07FilePath;
            _gamePathSettings.Th08 = GamePath.Th08FilePath;
            _gamePathSettings.Th09 = GamePath.Th09FilePath;
            _gamePathSettings.Th10 = GamePath.Th10FilePath;
            _gamePathSettings.Th11 = GamePath.Th11FilePath;
            _gamePathSettings.Th12 = GamePath.Th12FilePath;
            _gamePathSettings.Th13 = GamePath.Th13FilePath;
            _gamePathSettings.Th14 = GamePath.Th14FilePath;
            _gamePathSettings.Th15 = GamePath.Th15FilePath;
            _gamePathSettings.Th16 = GamePath.Th16FilePath;
            _gamePathSettings.Th17 = GamePath.Th17FilePath;
            _gamePathSettings.Th18 = GamePath.Th18FilePath;

            if (!string.IsNullOrEmpty(gamePathSettingsFile))
            {
                // XmlSerializerを使ってファイルに保存（SettingSerializerオブジェクトの内容を書き込む）
                XmlSerializer gamePathSettingsSerializer = new(typeof(GamePathSettings));
                FileStream fileStream = new(gamePathSettingsFile, FileMode.Create);
                // オブジェクトをシリアル化してXMLファイルに書き込む
                gamePathSettingsSerializer.Serialize(fileStream, _gamePathSettings);
                fileStream.Close();
            }
        }

        public static void ConfigureGamePathSettings()
        {
            string? gamePathSettingsFile = PathInfo.GamePathSettingsFile;
            if (!string.IsNullOrEmpty(gamePathSettingsFile) && File.Exists(gamePathSettingsFile))
            {
                XmlSerializer gamePathSettingsSerializer = new(typeof(GamePathSettings));
                FileStream fileStream = new(gamePathSettingsFile, FileMode.Open);

                _gamePathSettings = (GamePathSettings)gamePathSettingsSerializer.Deserialize(fileStream);
                fileStream.Close();

                GamePath.Th06FilePath = _gamePathSettings.Th06;
                GamePath.Th07FilePath = _gamePathSettings.Th07;
                GamePath.Th08FilePath = _gamePathSettings.Th08;
                GamePath.Th09FilePath = _gamePathSettings.Th09;
                GamePath.Th10FilePath = _gamePathSettings.Th10;
                GamePath.Th11FilePath = _gamePathSettings.Th11;
                GamePath.Th12FilePath = _gamePathSettings.Th12;
                GamePath.Th13FilePath = _gamePathSettings.Th13;
                GamePath.Th14FilePath = _gamePathSettings.Th14;
                GamePath.Th15FilePath = _gamePathSettings.Th15;
                GamePath.Th16FilePath = _gamePathSettings.Th16;
                GamePath.Th17FilePath = _gamePathSettings.Th17;
                GamePath.Th18FilePath = _gamePathSettings.Th18;
            }
            else
            {
                GamePath.Th06FilePath = string.Empty;
                GamePath.Th07FilePath = string.Empty;
                GamePath.Th08FilePath = string.Empty;
                GamePath.Th09FilePath = string.Empty;
                GamePath.Th10FilePath = string.Empty;
                GamePath.Th11FilePath = string.Empty;
                GamePath.Th12FilePath = string.Empty;
                GamePath.Th13FilePath = string.Empty;
                GamePath.Th14FilePath = string.Empty;
                GamePath.Th15FilePath = string.Empty;
                GamePath.Th16FilePath = string.Empty;
                GamePath.Th17FilePath = string.Empty;
                GamePath.Th18FilePath = string.Empty;
            }
        }

        public static void SaveApplicationSettings(ApplicationSettings applicationSettings)
        {
            string? applicationSettingsFile = PathInfo.ApplicationSettingsFile;

            XmlSerializer applicationSettingsSerializer = new(typeof(ApplicationSettings));
            FileStream fileStream = new(applicationSettingsFile, FileMode.Create);
            applicationSettingsSerializer.Serialize(fileStream, applicationSettings);
            fileStream.Close();
        }

        public static ApplicationSettings ConfigureApplicationSettings()
        {
            string? applicationSettingsFile = PathInfo.ApplicationSettingsFile;

            ApplicationSettings applicationSettings = new();

            if (File.Exists(applicationSettingsFile))
            {
                XmlSerializer applicationSettingsSerializer = new(typeof(ApplicationSettings));
                FileStream fileStream = new(applicationSettingsFile, FileMode.Open);

                applicationSettings = (ApplicationSettings)applicationSettingsSerializer.Deserialize(fileStream);
                fileStream.Close();
            }
            else
            {
                applicationSettings.MainWindowWidth = 550;
                applicationSettings.MainWindowHeight = 400;
                applicationSettings.SelectedGameId = GameIndex.Th06;
                applicationSettings.ResizeRateIndex = 2;
                applicationSettings.ResizeByRate = true;
                applicationSettings.ThemeName = "Light";
                applicationSettings.CaptureFileFormat = "BMP";
                applicationSettings.CaptureFileDirectory = PathInfo.GameCaptureDirectory;
            }

            return applicationSettings;
        }
    }
}
