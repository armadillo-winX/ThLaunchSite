using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ThLaunchSite.Settings
{
    internal class SettingsConfiguration
    {
        public static void SaveGamePathSettings()
        {
            string? gamePathSettingsFile = PathInfo.GamePathSettingsFile;

            List<string> allGamesList = GameIndex.GetAllGamesList();

            XmlDocument gamePathSettingsXml = new();

            XmlNode docNode = gamePathSettingsXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            _ = gamePathSettingsXml.AppendChild(docNode);

            XmlNode rootNode = gamePathSettingsXml.CreateElement("GamePathSettings");
            _ = gamePathSettingsXml.AppendChild(rootNode);

            foreach (string gameId in allGamesList)
            {
                string gameFilePath = GameFile.GetGameFilePath(gameId);
                XmlElement gamePathConfigNode = gamePathSettingsXml.CreateElement(gameId);
                gamePathConfigNode.InnerText = gameFilePath ?? string.Empty; //nullチェック

                _ = rootNode.AppendChild(gamePathConfigNode);
            }

            gamePathSettingsXml.Save(gamePathSettingsFile);
        }

        public static void ConfigureGamePathSettings()
        {
            string? gamePathSettingsFile = PathInfo.GamePathSettingsFile;

            List<string> allGamesList = GameIndex.GetAllGamesList();

            if (!string.IsNullOrEmpty(gamePathSettingsFile) && File.Exists(gamePathSettingsFile))
            {
                XmlDocument gamePathSettingsXml = new();
                gamePathSettingsXml.Load(gamePathSettingsFile);

                XmlElement rootNode = gamePathSettingsXml.DocumentElement;

                foreach (string gameId in allGamesList)
                {
                    XmlNode gamePathConfigNode = rootNode.SelectSingleNode(gameId);
                    if (gamePathConfigNode != null)
                    {
                        GameFile.SetGameFilePath(gameId, gamePathConfigNode.InnerText);
                    }
                    else
                    {
                        GameFile.SetGameFilePath(gameId, string.Empty);
                    }
                }
            }
            else
            {
                foreach (string gameId in allGamesList)
                {
                    GameFile.SetGameFilePath(gameId, string.Empty);
                }
            }
        }

        public static void SaveGameEditionSettings()
        {
            string? gameEditionSettingsFile = PathInfo.GameEditionSettingsFile;

            List<string> allGamesList = GameIndex.GetAllGamesList();

            XmlDocument gameEditionSettingsXml = new();

            XmlNode docNode = gameEditionSettingsXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            _ = gameEditionSettingsXml.AppendChild(docNode);

            XmlNode rootNode = gameEditionSettingsXml.CreateElement("GameEditionSettings");
            _ = gameEditionSettingsXml.AppendChild(rootNode);

            foreach (string gameId in allGamesList)
            {
                string gameEdition = GameIndex.GetGameEdition(gameId);
                XmlElement gameEditionConfigNode = gameEditionSettingsXml.CreateElement(gameId);
                gameEditionConfigNode.InnerText = gameEdition ?? string.Empty;

                _ = rootNode.AppendChild(gameEditionConfigNode);
            }

            gameEditionSettingsXml.Save(gameEditionSettingsFile);
        }

        public static void ConfigureGameEditionSettings()
        {
            string? gameEditionSettingsFile = PathInfo.GameEditionSettingsFile;

            List<string> allGamesList = GameIndex.GetAllGamesList();

            if (!string.IsNullOrEmpty(gameEditionSettingsFile) && File.Exists(gameEditionSettingsFile))
            {
                XmlDocument gameEditionSettingsXml = new();
                gameEditionSettingsXml.Load(gameEditionSettingsFile);

                XmlElement rootNode = gameEditionSettingsXml.DocumentElement;

                foreach (string gameId in allGamesList)
                {
                    XmlNode gameEditionConfigNode = rootNode.SelectSingleNode(gameId);
                    if (gameEditionConfigNode != null)
                    {
                        GameIndex.SetGameEdition(gameId, gameEditionConfigNode.InnerText);
                    }
                    else
                    {
                        GameIndex.SetGameEdition(gameId, "Product");
                    }
                }
            }
            else
            {
                foreach (string gameId in allGamesList)
                {
                    GameIndex.SetGameEdition(gameId, "Product");
                }
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
                applicationSettings.SelectedGameId = string.Empty;
                applicationSettings.SelectedGamePriorityIndex = 0;
                applicationSettings.ResizeRateIndex = 2;
                applicationSettings.ResizeByRate = true;
                applicationSettings.ThemeName = "Light";
                applicationSettings.LanguageId = "Auto";
                applicationSettings.CaptureFileFormat = "BMP";
                applicationSettings.CaptureFileDirectory = PathInfo.GameCaptureDirectory;
            }

            return applicationSettings;
        }
    }
}
