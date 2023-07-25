global using System;
global using System.Diagnostics;
global using System.IO;
global using System.Windows;

global using ThLaunchSite.Exceptions;
global using ThLaunchSite.Game;
global using ThLaunchSite.Settings;

using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ThLaunchSite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AboutDialog? _aboutDialog = null;

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

            SettingsConfiguration.SaveMainWindowSettings(mainWindowSettings);
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
            GameOperation.LaunchGame(gameId);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
    }
}
