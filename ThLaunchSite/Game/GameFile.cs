namespace ThLaunchSite.Game
{
    internal class GameFile
    {
        public static string? GetGameOperationLogFilePath(string gameId)
        {
            string gamePath = GamePath.GetGamePath(gameId);
            if (gamePath == null)
            {
                return null;
            }
            else
            {
                string gameDirectory = Path.GetDirectoryName(gamePath);

                if (
                    gameId == GameIndex.Th06 ||
                    gameId == GameIndex.Th07 ||
                    gameId == GameIndex.Th08 ||
                    gameId == GameIndex.Th09 ||
                    gameId == GameIndex.Th10 ||
                    gameId == GameIndex.Th11 ||
                    gameId == GameIndex.Th12)
                {
                    return $"{gameDirectory}\\log.txt";
                }
                else
                {
                    return $"{PathInfo.ShanghaiAliceAppData}\\{gameId.ToLower()}\\log.txt";
                }
            }
        }
    }
}
