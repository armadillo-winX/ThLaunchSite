using System.Threading;

namespace ThLaunchSite.Game
{
    internal class GameProcessHandler
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
                    WorkingDirectory = gameDirectory,
                    UseShellExecute = true
                };

                _ = Process.Start(gameProcessStartInfo);

                int i = 0;
                while (!IsRunningGame(gameProcessName))
                {
                    Thread.Sleep(100);
                    i++;
                    if (i == 100)
                    {
                        throw new ProcessNotFoundException(Properties.Resources.ErrorMessageFailedToDetectGameProcess);
                    }
                }

                return gameProcessName;
            }
            else
            {
                throw new FileNotFoundException(Properties.Resources.ErrorMessageGameFileNotFound);
            }
        }

        public static string StartGameProcessWithApplyingTool(string gameId, string toolName)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);
            string patchPath = $"{gameDirectory}\\{toolName}";
            if (File.Exists(gamePath) && File.Exists(patchPath))
            {
                string gameProcessName = Path.GetFileNameWithoutExtension(gamePath);

                ProcessStartInfo gameProcessStartInfo = new()
                {
                    FileName = patchPath,
                    WorkingDirectory = gameDirectory,
                    UseShellExecute = true
                };

                _ = Process.Start(gameProcessStartInfo);

                int i = 0;
                while (!IsRunningGame(gameProcessName))
                {
                    Thread.Sleep(100);
                    i++;
                    if (i == 100)
                    {
                        throw new ProcessNotFoundException(Properties.Resources.ErrorMessageFailedToDetectGameProcess);
                    }
                }

                return gameProcessName;
            }
            else if (!File.Exists(patchPath))
            {
                throw new FileNotFoundException($"{toolName}{Properties.Resources.ErrorMessageNotFound}");
            }
            else
            {
                throw new FileNotFoundException(Properties.Resources.ErrorMessageGameFileNotFound);
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
                throw new FileNotFoundException(Properties.Resources.ErrorMessageCustomProgramNotFound);
            }
            else
            {
                throw new FileNotFoundException(Properties.Resources.ErrorMessageGameFileNotFound);
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
                throw new ProcessNotFoundException(Properties.Resources.ErrorMessageFailedToDetectGameProcess);
            }
        }

        public static bool IsRunningGame(string gameProcessName)
        {
            if (string.IsNullOrEmpty(gameProcessName))
            {
                return false;
            }
            else
            {
                return Process.GetProcessesByName(gameProcessName).Length > 0;
            }
        }

        public static void SetGamePriority(string gameProcessName, int gamePriorityIndex)
        {
            Process[] gameProcesses = Process.GetProcessesByName(gameProcessName);
            Process gameProcess = gameProcesses[0];
            if (gamePriorityIndex == 1)
            {
                gameProcess.PriorityClass = ProcessPriorityClass.High;
            }
            else if (gamePriorityIndex == 2)
            {
                gameProcess.PriorityClass = ProcessPriorityClass.AboveNormal;
            }
            else if (gamePriorityIndex == 3)
            {
                gameProcess.PriorityClass = ProcessPriorityClass.Normal;
            }
            else if (gamePriorityIndex == 4)
            {
                gameProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
            }
            else if (gamePriorityIndex == 5)
            {
                gameProcess.PriorityClass = ProcessPriorityClass.Idle;
            }
        }
    }
}
