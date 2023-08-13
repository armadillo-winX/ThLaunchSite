using System.Reflection;
using System.Threading;

namespace ThLaunchSite.Game
{
    internal class GameOperation
    {
        public static string StartGameProcess(string gameId)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            if (File.Exists(gamePath))
            {
                string gameProcessName = Path.GetFileNameWithoutExtension(gamePath);
                string gameDirectory = Path.GetDirectoryName(gamePath);

                ProcessStartInfo gameProcessStartInfo = new()
                {
                    FileName = gamePath,
                    WorkingDirectory = gameDirectory
                };

                _ = Process.Start(gameProcessStartInfo);

                int i = 0;
                while (!IsRunningGame(gameProcessName))
                {
                    Thread.Sleep(100);
                    i++;
                    if (i == 100)
                    {
                        throw new ProcessNotFoundException("ゲームプロセスの検知に失敗しました。");
                    }
                }

                return gameProcessName;
            }
            else
            {
                throw new FileNotFoundException("ゲームの実行ファイルが見つかりませんでした。");
            }
        }

        public static string StartGameProcessWithPatch(string gameId, string patchName)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);
            string patchPath = $"{gameDirectory}\\{patchName}";
            if (File.Exists(gamePath) && File.Exists(patchPath))
            {
                string gameProcessName = Path.GetFileNameWithoutExtension(gamePath);

                ProcessStartInfo gameProcessStartInfo = new()
                {
                    FileName = patchPath,
                    WorkingDirectory = gameDirectory
                };

                _ = Process.Start(gameProcessStartInfo);

                int i = 0;
                while (!IsRunningGame(gameProcessName))
                {
                    Thread.Sleep(100);
                    i++;
                    if (i == 100)
                    {
                        throw new ProcessNotFoundException("ゲームプロセスの検知に失敗しました。");
                    }
                }

                return gameProcessName;
            }
            else if (!File.Exists(patchPath))
            {
                throw new FileNotFoundException($"{patchName}が見つかりませんでした。");
            }
            else
            {
                throw new FileNotFoundException("ゲームの実行ファイルが見つかりませんでした。");
            }
        }

        public static void StartCustomProgramProcess(string gameId)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);
            string customProgramPath = $"{gameDirectory}\\custom.exe";
            if (File.Exists(gamePath) && File.Exists(customProgramPath))
            {
                ProcessStartInfo customProgramStartInfo = new()
                {
                    FileName = customProgramPath,
                    WorkingDirectory = gameDirectory
                };

                _ = Process.Start(customProgramStartInfo);
            }
            else if (!File.Exists(customProgramPath))
            {
                throw new FileNotFoundException("環境カスタムプログラム(custom.exe)が見つかりませんでした。");
            }
            else
            {
                throw new FileNotFoundException("ゲームの実行ファイルが見つかりませんでした。");
            }
        }

        public static void OpenGameDirectory(string gameId)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);
            if (Directory.Exists(gameDirectory))
            {
                _ = Process.Start("explorer.exe", gameDirectory);
            }
        }

        public static string SearchRunningGameProcess()
        {
            PropertyInfo[] propertyInfos = typeof(GameIndex).GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                string gameId = propertyInfo.GetValue(null, null).ToString();
                string gamePath = GameFile.GetGameFilePath(gameId);
                string gameProcessName = Path.GetFileNameWithoutExtension(gamePath);
                if (IsRunningGame(gameProcessName))
                {
                    return gameId;
                }
            }

            throw new ProcessNotFoundException("ゲームプロセスが検知されませんでした。");
        }

        public static void KillGameProcess(string gameProcessName)
        {
            Process[] gameProcesses = Process.GetProcessesByName(gameProcessName);
            if (gameProcesses.Length > 0)
            {
                foreach (Process gameProcess in gameProcesses)
                {
                    gameProcess.Kill();
                    //終了を待機
                    gameProcess.WaitForExit();
                }
            }
            else
            {
                throw new ProcessNotFoundException("ゲームプロセスの検知に失敗しました。");
            }
        }

        public static bool IsRunningGame(string gameProcessName)
        {
            if (string.IsNullOrEmpty(gameProcessName))
            {
                return false;
            }
            else if (Process.GetProcessesByName(gameProcessName).Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
