namespace ThLaunchSite.Game
{
    internal class GameScoreBackup
    {
        public static bool CreateScoreBackup(string gameId)
        {
            string scoreFilePath = GameFile.GetScoreFilePath(gameId);
            string backupDirectory = $"{PathInfo.ScoreBackupDirectory}\\{gameId}";

            if (File.Exists(scoreFilePath))
            {
                if (!Directory.Exists(backupDirectory))
                    Directory.CreateDirectory(backupDirectory);

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                File.Copy(scoreFilePath, $"{backupDirectory}\\{timestamp}.bak", true);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RestoreFromScoreBackup(string gameId, string backupFileName)
        {
            string scoreFilePath = GameFile.GetScoreFilePath(gameId);
            string backupFilePath =
                $"{PathInfo.ScoreBackupDirectory}\\{gameId}\\{backupFileName}";

            File.Copy(backupFilePath, scoreFilePath , true);
        }

        public static string[] GetScoreBackupFiles(string gameId)
        {
            string backupDirectory = $"{PathInfo.ScoreBackupDirectory}\\{gameId}";
            string[] scoreBackupFiles = Directory.GetFiles(backupDirectory, "*.bak", SearchOption.TopDirectoryOnly);

            return scoreBackupFiles;
        }

        public static void DeleteScoreBackupFile(string gameId, string backupFileName)
        {
            string backupFilePath = $"{PathInfo.ScoreBackupDirectory}\\{gameId}\\{backupFileName}";
            File.Delete(backupFilePath);
        }
    }
}
