using System.Xml.Serialization;

namespace ThLaunchSite.Settings
{
    internal class SettingsConfiguration
    {
        public static GamePathSettings? _gamePathSettings = new();

        public static void SaveGamePathSettings()
        {
            string? gamePathSettingsFile = PathInfo.GamePathSettingsFile;

            _gamePathSettings.Th06 = GamePath.Th06;
            _gamePathSettings.Th07 = GamePath.Th07;
            _gamePathSettings.Th08 = GamePath.Th08;
            _gamePathSettings.Th09 = GamePath.Th09;
            _gamePathSettings.Th10 = GamePath.Th10;
            _gamePathSettings.Th11 = GamePath.Th11;
            _gamePathSettings.Th12 = GamePath.Th12;
            _gamePathSettings.Th13 = GamePath.Th13;
            _gamePathSettings.Th14 = GamePath.Th14;
            _gamePathSettings.Th15 = GamePath.Th15;
            _gamePathSettings.Th16 = GamePath.Th16;
            _gamePathSettings.Th17 = GamePath.Th17;
            _gamePathSettings.Th18 = GamePath.Th18;

            if (!string.IsNullOrEmpty(gamePathSettingsFile))
            {
                // XmlSerializerを使ってファイルに保存（SettingSerializerオブジェクトの内容を書き込む）
                XmlSerializer serializer = new(typeof(GamePathSettings));
                FileStream fs = new(gamePathSettingsFile, FileMode.Create);
                // オブジェクトをシリアル化してXMLファイルに書き込む
                serializer.Serialize(fs, _gamePathSettings);
                fs.Close();
            }
        }

        public static void ConfigureGamePathSettings()
        {
            string? gamePathSettingsFile = PathInfo.GamePathSettingsFile;
            if (!string.IsNullOrEmpty(gamePathSettingsFile) && File.Exists(gamePathSettingsFile))
            {
                XmlSerializer serializer = new(typeof(GamePathSettings));
                FileStream fs = new(gamePathSettingsFile, FileMode.Open);

                _gamePathSettings = (GamePathSettings)serializer.Deserialize(fs);
                fs.Close();

                GamePath.Th06 = _gamePathSettings.Th06;
                GamePath.Th07 = _gamePathSettings.Th07;
                GamePath.Th08 = _gamePathSettings.Th08;
                GamePath.Th09 = _gamePathSettings.Th09;
                GamePath.Th10 = _gamePathSettings.Th10;
                GamePath.Th11 = _gamePathSettings.Th11;
                GamePath.Th12 = _gamePathSettings.Th12;
                GamePath.Th13 = _gamePathSettings.Th13;
                GamePath.Th14 = _gamePathSettings.Th14;
                GamePath.Th15 = _gamePathSettings.Th15;
                GamePath.Th16 = _gamePathSettings.Th16;
                GamePath.Th17 = _gamePathSettings.Th17;
                GamePath.Th18 = _gamePathSettings.Th18;
            }
            else
            {
                GamePath.Th06 = string.Empty;
                GamePath.Th07 = string.Empty;
                GamePath.Th08 = string.Empty;
                GamePath.Th09 = string.Empty;
                GamePath.Th10 = string.Empty;
                GamePath.Th11 = string.Empty;
                GamePath.Th12 = string.Empty;
                GamePath.Th13 = string.Empty;
                GamePath.Th14 = string.Empty;
                GamePath.Th15 = string.Empty;
                GamePath.Th16 = string.Empty;
                GamePath.Th17 = string.Empty;
                GamePath.Th18 = string.Empty;
            }
        }

        public static void SaveMainWindowSettings(MainWindowSettings mainWindowSettings)
        {
            string? mainWindowSettingsFile = PathInfo.MainWindowSettingsFile;

            XmlSerializer mainWindowSettingsSerializer = new(typeof(MainWindowSettings));
            FileStream fs = new(mainWindowSettingsFile, FileMode.Create);
            mainWindowSettingsSerializer.Serialize(fs, mainWindowSettings);
            fs.Close();
        }

        public static MainWindowSettings ConfigureMainWindowSettings()
        {
            string? mainWindowSettingsFile = PathInfo.MainWindowSettingsFile;

            MainWindowSettings mainWindowSettings = new();

            if (File.Exists(mainWindowSettingsFile))
            {
                XmlSerializer mainWindowSettingsSerializer = new(typeof(MainWindowSettings));
                FileStream fs = new(mainWindowSettingsFile, FileMode.Open);

                mainWindowSettings = (MainWindowSettings)mainWindowSettingsSerializer.Deserialize(fs);
                fs.Close();
            }
            else
            {
                mainWindowSettings.WindowWidth = 550;
                mainWindowSettings.WindowHeight = 400;
                mainWindowSettings.SelectedGameId = GameIndex.Th06;
            }

            return mainWindowSettings;
        }
    }
}
