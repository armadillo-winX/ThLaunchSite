namespace ThLaunchSite
{
    internal class User
    {
        private static readonly string _usersDirectory = PathInfo.UsersDirectory;
        private static readonly string _userIndex = PathInfo.UserIndex;
        private static readonly string _userSelectionConfig = PathInfo.UserSelectionConfig;

        public static string? CurrentUserName { get; set; }

        public static string? CurrentUserDirectoryName { get; set; }

        public static void AddUser(string userName)
        {
            if (!File.Exists(_userIndex))
            {
                CreateUserIndex();
            }

            int i = 1;
            string newUserDirectory = $"user{i}";
            while (Directory.Exists($"{_usersDirectory}\\{newUserDirectory}"))
            {
                i++;
                newUserDirectory = $"user{i}";
            }

            Directory.CreateDirectory($"{_usersDirectory}\\{newUserDirectory}");

            XmlDocument doc = new();
            doc.Load(_userIndex);

            XmlElement rootNode = doc.DocumentElement;

            XmlElement userElement = doc.CreateElement("User");
            XmlAttribute index = doc.CreateAttribute("Index");
            index.Value = userName;
            _ = userElement.Attributes.Append(index);
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
            _ = rootNode.AppendChild(userElement);
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。

            XmlElement nameElement = doc.CreateElement("Name");
            _ = nameElement.AppendChild(doc.CreateTextNode(userName));
            _ = userElement.AppendChild(nameElement);

            XmlElement pathElement = doc.CreateElement("DirectoryName");
            _ = pathElement.AppendChild(doc.CreateTextNode(newUserDirectory));
            _ = userElement.AppendChild(pathElement);

            doc.Save(_userIndex);

            CurrentUserName = userName;
            CurrentUserDirectoryName = newUserDirectory;
        }

        public static bool Exist(string userName)
        {
            if (File.Exists(_userIndex))
            {
                XmlDocument doc = new();
                doc.Load(_userIndex);

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
                XmlNode node = doc.DocumentElement.SelectSingleNode($"//User[@Index='{userName}']");
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。

                return node != null;
            }
            else
            {
                return false;
            }
        }

        public static void CreateUserIndex()
        {
            if (!Directory.Exists(_usersDirectory))
            {
                Directory.CreateDirectory(_usersDirectory);
            }

            if (!File.Exists(_userIndex))
            {
                XmlDocument doc = new();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                _ = doc.AppendChild(docNode);

                XmlNode rootNode = doc.CreateElement("UserIndex");
                _ = doc.AppendChild(rootNode);
                doc.Save(_userIndex);
            }
        }

        public static string? GetUserDirectoryName(string userName)
        {
            if (Exist(userName))
            {
                XmlDocument doc = new();
                doc.Load(_userIndex);

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
                string userDirectoryName = doc.SelectSingleNode($"//User[@Index='{userName}']/DirectoryName").InnerText;
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。

                return userDirectoryName;
            }
            else
            {
                return null;
            }
        }

        public static void SwitchUser(string? userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                string userDirectoryName = GetUserDirectoryName(userName);
                CurrentUserName = userName;
                CurrentUserDirectoryName = userDirectoryName;
            }
        }

        public static void SaveUserSelectionConfig()
        {
            if (!string.IsNullOrEmpty(CurrentUserName))
            {
                XmlDocument doc = new();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                _ = doc.AppendChild(docNode);

                XmlNode rootNode = doc.CreateElement("UserSelectionConfig");
                _ = doc.AppendChild(rootNode);

                XmlElement selectionNode = doc.CreateElement("UserSelection");
                _ = selectionNode.AppendChild(doc.CreateTextNode(CurrentUserName));
                _ = rootNode.AppendChild(selectionNode);
                doc.Save(_userSelectionConfig);
            }
        }

        public static string GetUserSelection()
        {
            XmlDocument doc = new();
            doc.Load(_userSelectionConfig);
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
            string userName = doc.SelectSingleNode("//UserSelection").InnerText;
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。

            return userName;
        }
    }
}
