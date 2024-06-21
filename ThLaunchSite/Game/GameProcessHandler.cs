namespace ThLaunchSite.Game
{
    internal class GameProcessHandler
    {
        public static Process StartGameProcess(string gameId)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            if (File.Exists(gamePath))
            {
                string gameDirectory = Path.GetDirectoryName(gamePath);

                ProcessStartInfo gameProcessStartInfo = new()
                {
                    FileName = gamePath,
                    WorkingDirectory = gameDirectory,
                    UseShellExecute = true
                };

                Process gameProcess = Process.Start(gameProcessStartInfo);

                gameProcess.WaitForInputIdle();

                return gameProcess;
            }
            else
            {
                throw new FileNotFoundException(Properties.Resources.ErrorMessageGameFileNotFound);
            }
        }

        public static Process StartGameProcessWithApplyingTool(string gameId, string toolName)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);
            string patchPath = $"{gameDirectory}\\{toolName}";
            if (File.Exists(gamePath) && File.Exists(patchPath))
            {
                ProcessStartInfo gameProcessStartInfo = new()
                {
                    FileName = patchPath,
                    WorkingDirectory = gameDirectory,
                    UseShellExecute = true
                };

                Process gameProcess = Process.Start(gameProcessStartInfo);

                gameProcess.WaitForInputIdle();

                return gameProcess;
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

        public static void KillGameProcess(Process? gameProcess)
        {
            if (gameProcess != null)
            {
                gameProcess.Kill();
                //終了を待機
                gameProcess.WaitForExit();
            }
            else
            {
                throw new ProcessNotFoundException(Properties.Resources.ErrorMessageFailedToDetectGameProcess);
            }
        }

        public static bool IsRunningGame(int gameProcessId)
        {
            try
            {
                Process.GetProcessById(gameProcessId);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static void SetGamePriority(Process gameProcess, int gamePriorityIndex)
        {
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
