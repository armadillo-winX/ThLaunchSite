using System.Collections.Generic;

namespace ThLaunchSite.Game
{
    internal class GameFile
    {
        public static string? GetGameFilePath(string gameId)
        {
            if (gameId == GameIndex.Th06)
            {
                return GamePath.Th06FilePath;
            }
            else if (gameId == GameIndex.Th07)
            {
                return GamePath.Th07FilePath;
            }
            else if (gameId == GameIndex.Th08)
            {
                return GamePath.Th08FilePath;
            }
            else if (gameId == GameIndex.Th09)
            {
                return GamePath.Th09FilePath;
            }
            else if (gameId == GameIndex.Th10)
            {
                return GamePath.Th10FilePath;
            }
            else if (gameId == GameIndex.Th11)
            {
                return GamePath.Th11FilePath;
            }
            else if (gameId == GameIndex.Th12)
            {
                return GamePath.Th12FilePath;
            }
            else if (gameId == GameIndex.Th13)
            {
                return GamePath.Th13FilePath;
            }
            else if (gameId == GameIndex.Th14)
            {
                return GamePath.Th14FilePath;
            }
            else if (gameId == GameIndex.Th15)
            {
                return GamePath.Th15FilePath;
            }
            else if (gameId == GameIndex.Th16)
            {
                return GamePath.Th16FilePath;
            }
            else if (gameId == GameIndex.Th17)
            {
                return GamePath.Th17FilePath;
            }
            else if (gameId == GameIndex.Th18)
            {
                return GamePath.Th18FilePath;
            }
            else
            {
                return null;
            }
        }

        public static void SetGameFilePath(string gameId, string gamePath)
        {
            if (gameId == GameIndex.Th06)
            {
                GamePath.Th06FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th07)
            {
                GamePath.Th07FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th08)
            {
                GamePath.Th08FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th09)
            {
                GamePath.Th09FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th10)
            {
                GamePath.Th10FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th11)
            {
                GamePath.Th11FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th12)
            {
                GamePath.Th12FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th13)
            {
                GamePath.Th13FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th14)
            {
                GamePath.Th14FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th15)
            {
                GamePath.Th15FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th16)
            {
                GamePath.Th16FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th17)
            {
                GamePath.Th17FilePath = gamePath;
            }
            else if (gameId == GameIndex.Th18)
            {
                GamePath.Th18FilePath = gamePath;
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
