global using System;
global using System.Diagnostics;
global using System.IO;
global using System.Windows;
global using System.Xml;

global using ThLaunchSite.Dialogs;
global using ThLaunchSite.Exceptions;
global using ThLaunchSite.Game;
global using ThLaunchSite.Settings;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

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
        private readonly string _usersDirectory = PathInfo.UsersDirectory;
        private readonly string _userIndex = PathInfo.UserIndex;
        private readonly string _userSelectionConfig = PathInfo.UserSelectionConfig;

        public MainWindow()
        {
            InitializeComponent();

            this.Title = $"{_appName} ver.{_appVersion}";

            if (!Directory.Exists(_usersDirectory) || !File.Exists(_userIndex))
            {
                AddUserDialog addUserDialog = new();
                addUserDialog.ShowDialog();
            }
            else
            {
                try
                {
                    if (File.Exists(_userSelectionConfig))
                    {
                        string userName = User.GetUserSelection();
                        User.SwitchUser(userName);
                    }
                    else
                    {
                        SelectUserDialog selectUserDialog = new();
                        if (selectUserDialog.ShowDialog() == true)
                        {
                            string selectedUserName = selectUserDialog.SelectedUserName;
                            User.SwitchUser(selectedUserName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void AddUserMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddUserDialog addUserDialog = new();
            addUserDialog.Owner = this;
            if (addUserDialog.ShowDialog() == true)
            {
                try
                {
                    User.SwitchUser(addUserDialog.UserName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SelectUserMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SelectUserDialog selectUserDialog = new();
            selectUserDialog.Owner = this;
            if (selectUserDialog.ShowDialog() == true)
            {
                try
                {
                    User.SwitchUser(selectUserDialog.SelectedUserName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                User.SaveUserSelectionConfig();
                SettingsConfiguration.SaveGamePathSettings();
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

            if (!string.IsNullOrEmpty(gamePath) )
            {
                GamePathBox.Text = gamePath;
            }
        }
    }
}
