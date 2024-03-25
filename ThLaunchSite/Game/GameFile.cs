using System.Reflection;

namespace ThLaunchSite.Game
{
    internal class GameFile
    {
        public static string? GetGameFilePath(string gameId)
        {
            //プロパティ名からプロパティを取得
            PropertyInfo? gamePathProperty = typeof(GamePath).GetProperty($"{gameId}FilePath");
            string? gamePath;
            if (gamePathProperty != null)
            {
                gamePath 
                = gamePathProperty.GetValue(null, null) != null ?
                gamePathProperty.GetValue(null, null).ToString() :
                string.Empty;
            }
            else
            {
                gamePath = string.Empty;
            }

            return gamePath;
        }

        public static void SetGameFilePath(string gameId, string gamePath)
        {
            //プロパティ名からプロパティを取得
            PropertyInfo? gamePathProperty = typeof(GamePath).GetProperty($"{gameId}FilePath");
            if (gamePathProperty != null)
            {
                //取得したプロパティに値を代入
                gamePathProperty.SetValue(null, gamePath);
            }
        }

        public static string[] GetThpracFiles(string gameId)
        {
            string? gamePath = GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);

            string[] thpracFiles = Directory.GetFiles(
                gameDirectory, "thprac*.exe", SearchOption.TopDirectoryOnly);

            return thpracFiles;
        }
    }
}
