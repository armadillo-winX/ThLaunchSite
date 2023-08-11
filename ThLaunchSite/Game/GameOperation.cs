using System.Reflection;
using System.Threading;

namespace ThLaunchSite.Game
{
    internal class GameOperation
    {
        public static string StartGameProcess(string gameId)
        {
            string? gamePath = GamePath.GetGamePath(gameId);
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

        public static string StartGameProcessWithVpatch(string gameId)
        {
            string? gamePath = GamePath.GetGamePath(gameId);
            string? gameDirectory = Path.GetDirectoryName(gamePath);
            string vpatchPath = $"{gameDirectory}\\vpatch.exe";
            if (File.Exists(gamePath) && File.Exists(vpatchPath))
            {
                string gameProcessName = Path.GetFileNameWithoutExtension(gamePath);

                ProcessStartInfo gameProcessStartInfo = new()
                {
                    FileName = vpatchPath,
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
            else if (!File.Exists(vpatchPath))
            {
                throw new FileNotFoundException("VsyncPatchの実行ファイル(vpatch.exe)が見つかりませんでした。");
            }
            else
            {
                throw new FileNotFoundException("ゲームの実行ファイルが見つかりませんでした。");
            }
        }

        public static string StartGameProcessWithThprac(string gameId)
        {
            string? gamePath = GamePath.GetGamePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
            string[] thpracFiles = Directory.GetFiles(
                gameDirectory, "thprac*.exe", SearchOption.TopDirectoryOnly);
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
            int thpracFilesLength = thpracFiles.Length;
            if (thpracFilesLength > 0 && File.Exists(gamePath))
            {
                string thpracPath = thpracFiles[thpracFilesLength - 1];

                string gameProcessName = Path.GetFileNameWithoutExtension(gamePath);

                ProcessStartInfo gameProcessStartInfo = new()
                {
                    FileName = thpracPath,
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
            else if (thpracFilesLength == 0)
            {
                throw new FileNotFoundException("thpracの実行ファイルが見つかりませんでした。");
            }
            else
            {
                throw new FileNotFoundException("ゲームの実行ファイルが見つかりませんでした。");
            }
        }

        public static string StartGameProcessWithAnyPatch(string gameId, string patch)
        {
            string? gamePath = GamePath.GetGamePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);
            string patchPath = $"{gameDirectory}\\{patch}";
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
                throw new FileNotFoundException("指定されたパッチの実行ファイルが見つかりませんでした。");
            }
            else
            {
                throw new FileNotFoundException("ゲームの実行ファイルが見つかりませんでした。");
            }
        }

        public static void LaunchCustomProgram(string gameId)
        {
            string? gamePath = GamePath.GetGamePath(gameId);
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
            string? gamePath = GamePath.GetGamePath(gameId);
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
                string gamePath = GamePath.GetGamePath(gameId);
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
