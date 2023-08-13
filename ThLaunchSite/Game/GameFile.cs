using System.Text;

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

        public static string? GetGameOperationLogData(string gameId)
        {
            string? gameOperationLogFilePath = GetGameOperationLogFilePath(gameId);

            if (File.Exists(gameOperationLogFilePath))
            {
                //Shift_JISに対応させる
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                StreamReader streamReader = new(gameOperationLogFilePath, Encoding.GetEncoding("shift_jis"));
                string gameOperationLogData = streamReader.ReadToEnd();
                streamReader.Close();

                return gameOperationLogData;
            }
            else
            {
                return "動作記録ファイルが見つかりませんでした。";
            }
        }
    }
}
