global using System;
global using System.Diagnostics;
global using System.IO;
global using System.Windows;

global using ThLaunchSite.Exceptions;
global using ThLaunchSite.Game;
global using ThLaunchSite.Settings;

using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ThLaunchSite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string GameProcessName { get; set; }

        private AboutDialog? _aboutDialog = null;
        private BackgroundWorker? _gameWaitingWorker = null;
        private DispatcherTimer? _gameControlTimer = null;

        private readonly string? _appName = VersionInfo.AppName;
        private readonly string? _appVersion = VersionInfo.AppVersion;
        private readonly string? _settingsDirectory = PathInfo.SettingsDirectory;

        private readonly Dictionary<string, int> GameDictionary =
            new()
            {
                { "Th06", 0 },
                { "Th07", 1 },
                { "Th08", 2 },
                { "Th09", 3 },
                { "Th10", 4 },
                { "Th11", 5 },
                { "Th12", 6 },
                { "Th13", 7 },
                { "Th14", 8 },
                { "Th15", 9 },
                { "Th16", 10 },
                { "Th17", 11 },
                { "Th18", 12 }
            };

        public MainWindow()
        {
            InitializeComponent();

            this.Title = $"{_appName} ver.{_appVersion}";
            if (((App)Application.Current).IsAdmin())
            {
                AuthorityBlock.Content = "管理者権限";
            }
            else
            {
                AuthorityBlock.Content = "ユーザー権限";
            }

            this.GameProcessName = string.Empty;

            EnableLimitationMode(false);

            if (!Directory.Exists(_settingsDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_settingsDirectory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"設定ファイルディレクトリの生成に失敗。\nアプリケーションを終了します。\n\n[詳細]\n{ex.Message}",
                        "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }

            try
            {
                SettingsConfiguration.ConfigureGamePathSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ゲームパス設定の構成に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                ConfigureMainWindowSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"メインウィンドウ設定の構成に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            AppStatusBlock.Content = "準備完了";
        }

        private string GetSelectedGameId()
        {
            if (GameComboBox.SelectedIndex > -1)
            {
                ComboBoxItem item = (ComboBoxItem)GameComboBox.SelectedItem;
                return (string)item.Uid;
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetSelectedGameTitle()
        {
            if (GameComboBox.SelectedIndex > -1)
            {
                ComboBoxItem item = (ComboBoxItem)GameComboBox.SelectedItem;
                return (string)item.Content;
            }
            else
            {
                return string.Empty;
            }
        }

        private async void LaunchGame(string gameId, int patchIndex)
        {
            try
            {
                EnableLimitationMode(true);

                string gameProcessName;

                switch (patchIndex)
                {
                    case 0:
                        gameProcessName = await Task.Run(() 
                            => GameOperation.StartGameProcess(gameId)
                            );
                        EnableWaitGameEndMode(gameProcessName);
                        break;
                    case 1:
                        gameProcessName = await Task.Run(() 
                            => GameOperation.StartGameProcessWithVpatch(gameId)
                            );
                        EnableWaitGameEndMode(gameProcessName);
                        break;
                    case 2:
                        gameProcessName = await Task.Run(()
                            => GameOperation.StartGameProcessWithThprac(gameId)
                            );
                        EnableWaitGameEndMode(gameProcessName);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ゲームの起動に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                EnableLimitationMode(false);
            }
        }

        private void ConfigureMainWindowSettings()
        {
            MainWindowSettings mainWindowSettings = SettingsConfiguration.ConfigureMainWindowSettings();
            this.Width = mainWindowSettings.WindowWidth;
            this.Height = mainWindowSettings.WindowHeight;
            this.Topmost = mainWindowSettings.AlwaysOnTop;
            AlwaysOnTopMenuItem.IsChecked = mainWindowSettings.AlwaysOnTop;
            ResizeRateComboBox.SelectedIndex = mainWindowSettings.ResizeRateIndex;
            if (mainWindowSettings.ResizeByRate) ResizeByRateRadioButton.IsChecked = true;
            if (mainWindowSettings.ResizeBySize) ResizeBySizeRadioButton.IsChecked = true;

            ResizeRateComboBox.IsEnabled = mainWindowSettings.ResizeByRate;
            GameWindowWidthBox.IsEnabled = mainWindowSettings.ResizeBySize;
            GameWindowHeightBox.IsEnabled = mainWindowSettings.ResizeBySize;
            GameWindowWidthBox.Text = mainWindowSettings.ResizeWidth;
            GameWindowHeightBox.Text = mainWindowSettings.ResizeHeight;

            string? selectedGameId = mainWindowSettings.SelectedGameId;
            if (!string.IsNullOrEmpty(selectedGameId))
            {
                GameComboBox.SelectedIndex = GameDictionary[selectedGameId];
            }
            else
            {
                GameComboBox.SelectedIndex = 0;
            }
        }

        private void SaveMainWindowSettings()
        {
            MainWindowSettings mainWindowSettings = new();
            mainWindowSettings.WindowWidth = this.Width;
            mainWindowSettings.WindowHeight = this.Height;
            mainWindowSettings.SelectedGameId = GetSelectedGameId();
            mainWindowSettings.AlwaysOnTop = AlwaysOnTopMenuItem.IsChecked;
            mainWindowSettings.ResizeRateIndex = ResizeRateComboBox.SelectedIndex;
            mainWindowSettings.ResizeByRate = ResizeByRateRadioButton.IsChecked == true;
            mainWindowSettings.ResizeBySize= ResizeBySizeRadioButton.IsChecked == true;
            mainWindowSettings.ResizeWidth = GameWindowWidthBox.Text;
            mainWindowSettings.ResizeHeight = GameWindowHeightBox.Text;

            SettingsConfiguration.SaveMainWindowSettings(mainWindowSettings);
        }

        private void EnableLimitationMode(bool enabled)
        {
            GameComboBox.IsEnabled = !enabled;
            LaunchGameMenuItem.IsEnabled = !enabled;
            LaunchWithVpatchMenuItem.IsEnabled = !enabled;
            LaunchWithThpracMenuItem.IsEnabled = !enabled;
            LaunchCustomProgramMenuItem.IsEnabled = !enabled;
            CatchGameProcessMenuItem.IsEnabled = !enabled;

            GamePathBox.IsEnabled= !enabled;
            GamePathBrowseButton.IsEnabled= !enabled;

            KillGameProcessMenuItem.IsEnabled = enabled;
            ResizeButton.IsEnabled= enabled;
        }

        private void EnableWaitGameEndMode(string gameProcessName)
        {
            this.GameProcessName= gameProcessName;
            AppStatusBlock.Content = "ゲームの終了を待機中";
            EnableLimitationMode(true);

            _gameWaitingWorker = new BackgroundWorker();
            _gameWaitingWorker.DoWork += new DoWorkEventHandler(Worker_Dowork);
            _gameWaitingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_Running_Complete);
            _gameWaitingWorker.RunWorkerAsync(gameProcessName);

            int time = 0;

            GameTitleBlock.Text = GetSelectedGameTitle();

            _gameControlTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _gameControlTimer.Tick += (e, s) =>
            {
                int minutes = time / 60;
                int secounds = time % 60; //timeを60で割ったあまり

                string gameRunTime = String.Format("{0:D2}min {1:D2}sec", minutes, secounds);
                GameRunningTimeBlock.Text = gameRunTime;
                time += 1;

                if (GameOperation.IsRunningGame(this.GameProcessName))
                {
                    Process[] gameProcesses = Process.GetProcessesByName(this.GameProcessName);
                    Process gameProcess = gameProcesses[0];
                    string pagedMemorySize = $"{gameProcess.WorkingSet64 / 1024 / 1024} MiB";
                    string virtualMemorySize = $"{gameProcess.VirtualMemorySize64 / 1024 / 1024} MiB";
                    PagedMemorySizeBlock.Text = pagedMemorySize;
                    VirtialMemorySizeBlock.Text = virtualMemorySize;
                }
            };
            _gameControlTimer.Start();
        }

        private void ShowCommandGameLauncherDialog()
        {
            if (_gameWaitingWorker != null && _gameWaitingWorker.IsBusy)
            {
                MessageBox.Show(this, "ゲーム終了待機中は他のゲームを起動することはできません。", _appName,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                CommandDialog commandDialog = new();
                commandDialog.Owner = this;
                if (commandDialog.ShowDialog() == true)
                {
                    string? gameId = commandDialog.GameId;
                    int patchIndex = commandDialog.PatchIndex;
                    if (!string.IsNullOrEmpty(gameId))
                    {
                        if (GameDictionary.ContainsKey(gameId))
                        {
                            int index = GameDictionary[gameId];
                            GameComboBox.SelectedIndex = index;
                            LaunchGame(gameId, patchIndex);
                        }
                        else
                        {
                            MessageBox.Show(this, "未対応のゲーム作品です。", "エラー",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void Worker_Dowork(object sender, DoWorkEventArgs e)
        {
            string name = (string)e.Argument;

            while (GameOperation.IsRunningGame(name))
            {
                Thread.Sleep(500);
            }
        }

        private void Worker_Running_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            _gameControlTimer.Stop();
            AppStatusBlock.Content = "準備完了";
            this.GameProcessName = string.Empty;
            EnableLimitationMode(false);
        }

        public void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_aboutDialog == null || !_aboutDialog.IsLoaded)
            {
                _aboutDialog = new();
                _aboutDialog.Owner = this;
                _aboutDialog.Show();
            }
            else
            {
                _aboutDialog.WindowState = WindowState.Normal;
                _ = _aboutDialog.Activate();
            }
        }

        private void GamePathBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "ゲーム実行ファイル|*.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                string gamePath = openFileDialog.FileName;
                GamePathBox.Text = gamePath;
                string gameId = GetSelectedGameId();
                GamePath.SetGamePath(gameId, gamePath);
            }
        }

        private void LaunchGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string gameId = GetSelectedGameId();
            LaunchGame(gameId, 0);
        }

        public void LaunchWithVpatchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string gameId = GetSelectedGameId();
            LaunchGame(gameId, 1);
        }

        public void LaunchWithThpracMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string gameId = GetSelectedGameId();
            LaunchGame(gameId, 2);
        }

        public void LaunchCustomeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = GetSelectedGameId();
                GameOperation.LaunchCustomProgram(gameId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"環境カスタムプログラムの起動に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void KillGameProcessMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = this.GameProcessName;
                GameOperation.KillGameProcess(gameId);
                MessageBox.Show(this, "ゲームを強制終了させました。", "ゲームプロセスの強制終了",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ゲームの強制終了に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CatchGameProcessMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string gameId = GetSelectedGameId();
            if (GameOperation.IsRunningGame(gameId))
            {
                EnableWaitGameEndMode(gameId);
            }
            else
            {
                MessageBox.Show(this, "ゲームプロセスが見つかりませんでした。", "ゲームプロセスの捕捉",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_gameWaitingWorker != null && _gameWaitingWorker.IsBusy)
            {
                MessageBoxResult result = MessageBox.Show(
                    this, $"ゲーム終了待機モードです。\n本当に{_appName}を終了させますか。", _appName,
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        SettingsConfiguration.SaveGamePathSettings();
                        SaveMainWindowSettings();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"設定の保存に失敗\n{ex.Message}", "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                try
                {
                    SettingsConfiguration.SaveGamePathSettings();
                    SaveMainWindowSettings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"設定の保存に失敗\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GamePathBox.Clear();

            string gameId = GetSelectedGameId();
            string? gamePath = GamePath.GetGamePath(gameId);

            if (!string.IsNullOrEmpty(gamePath))
            {
                GamePathBox.Text = gamePath;
            }
        }

        private void AlwaysOnTopMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = AlwaysOnTopMenuItem.IsChecked;
        }

        private void ResizeByRateRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ResizeRateComboBox.IsEnabled = ResizeByRateRadioButton.IsChecked == true;
            GameWindowWidthBox.IsEnabled = ResizeBySizeRadioButton.IsChecked == true;
            GameWindowHeightBox.IsEnabled = ResizeBySizeRadioButton.IsChecked == true;
        }

        private void ResizeBySizeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ResizeRateComboBox.IsEnabled = ResizeByRateRadioButton.IsChecked == true;
            GameWindowWidthBox.IsEnabled = ResizeBySizeRadioButton.IsChecked == true;
            GameWindowHeightBox.IsEnabled = ResizeBySizeRadioButton.IsChecked == true;
        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            string name = this.GameProcessName;

            if (ResizeByRateRadioButton.IsChecked == true)
            {
                if (ResizeRateComboBox.SelectedIndex > -1)
                {
                    ComboBoxItem item = (ComboBoxItem)ResizeRateComboBox.SelectedItem;
                    string rate = (string)item.Content;

                    double resizeRate = double.Parse(rate.Replace("%", "")) / 100;

                    try
                    {
                        int[] sizes = GameWindowHandler.GetWindowSizes(name);

                        //リサイズ率に基づいてウィンドウの変更サイズを決定
                        int width = (int)Math.Round(sizes[0] * resizeRate);
                        int height = (int)Math.Round(sizes[1] * resizeRate);

                        GameWindowHandler.ResizeWindow(name, width, height);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (ResizeBySizeRadioButton.IsChecked == true)
            {
                try
                {
                    int width = int.Parse(GameWindowWidthBox.Text);
                    int height = int.Parse(GameWindowHeightBox.Text);

                    GameWindowHandler.ResizeWindow(name, width, height);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.P)
            {
                ShowCommandGameLauncherDialog();
            }
        }

        private void LaunchGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LaunchGameButtonMenu.IsOpen)
            {
                LaunchGameButtonMenu.PlacementTarget = LaunchGameButton;
                LaunchGameButtonMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                LaunchGameButtonMenu.IsOpen = true;
            }
            else
            {
                LaunchGameButtonMenu.IsOpen = false;
            }
        }

        private void CommandGameLauncherMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowCommandGameLauncherDialog();
        }
    }
}
