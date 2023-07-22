global using System;
global using System.Diagnostics;
global using System.IO;
global using System.Windows;

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

        public MainWindow()
        {
            InitializeComponent();

            this.Title = $"{_appName} ver.{_appVersion}";
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
