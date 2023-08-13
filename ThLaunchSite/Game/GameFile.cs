using System.Text;

namespace ThLaunchSite.Game
{
    internal class GameFile
    {
        public static string? GetGameOperationLogData(string gameId)
        {
            string? gameOperationLogFilePath = GamePath.GetGameOperationLogFilePath(gameId);

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
