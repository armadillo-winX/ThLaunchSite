using System.Collections.Generic;

namespace ThLaunchSite.Game
{
    internal class GameFile
    {
        private static Dictionary<string, string>? _gameFilesDictionary;

        public static string? GetGameFilePath(string gameId)
        {
            if (_gameFilesDictionary == null)
            {
                return string.Empty;
            }
            else
            {
                if (_gameFilesDictionary.TryGetValue(gameId, out string gameFilePath))
                {
                    return gameFilePath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static void SetGameFilePath(string gameId, string gameFilePath)
        {
            if (_gameFilesDictionary == null)
            {
                _gameFilesDictionary = [];
                _gameFilesDictionary.Add(gameId, gameFilePath);
            }
            else
            {
                if (_gameFilesDictionary.ContainsKey(gameId))
                {
                    _gameFilesDictionary[gameId] = gameFilePath;
                }
                else
                {
                    _gameFilesDictionary.Add(gameId, gameFilePath);
                }
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

        public static string? GetScoreFilePath(string gameId)
        {
            string gamePath = GetGameFilePath(gameId);
            if (gamePath == null)
            {
                return null;
            }
            else
            {
                string gameDirectory = Path.GetDirectoryName(gamePath);

                if (
                    gameId == "Th06" ||
                    gameId == "Th07" ||
                    gameId == "Th08" ||
                    gameId == "Th09")
                {
                    return $"{gameDirectory}\\score.dat";
                }
                else if (
                    gameId == "Th095" ||
                    gameId == "Th10" ||
                    gameId == "Th11" ||
                    gameId == "Th12")
                {
                    return $"{gameDirectory}\\score{gameId.ToLower()}.dat";
                }
                else
                {
                    string gameEdition = GameIndex.GetGameEdition(gameId);
                    if (gameEdition == "Trial")
                    {
                        return $"{PathInfo.ShanghaiAliceAppData}\\{gameId.ToLower()}tr\\score{gameId.ToLower()}.dat";
                    }
                    else
                    {
                        return $"{PathInfo.ShanghaiAliceAppData}\\{gameId.ToLower()}\\score{gameId.ToLower()}.dat";
                    }
                }
            }
        }
    }
}
