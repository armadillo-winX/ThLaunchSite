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
using System.Windows.Controls;

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
            try
            {
                string gameId = GetSelectedGameId();
                string gameProcessName = GameOperation.LaunchGame(gameId);
                EnableWaitGameEndMode(gameProcessName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ゲームの起動に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LaunchWithVpatchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = GetSelectedGameId();
                string gameProcessName = GameOperation.LaunchGameWithVpatch(gameId);
                EnableWaitGameEndMode(gameProcessName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ゲームの起動に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LaunchWithThpracMenuItem_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = GetSelectedGameId();
                string gameProcessName = GameOperation.LaunchGameWithThprac(gameId);
                EnableWaitGameEndMode(gameProcessName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ゲームの起動に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
    }
}
