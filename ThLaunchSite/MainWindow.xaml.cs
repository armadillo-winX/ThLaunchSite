global using System;
global using System.Diagnostics;
global using System.IO;
global using System.Windows;
global using System.Xml;

global using ThLaunchSite.Game;
global using ThLaunchSite.Exceptions;

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
using ThLaunchSite.Dialogs;

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
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
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
    }
}
