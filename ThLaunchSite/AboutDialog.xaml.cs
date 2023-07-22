using System.Windows;

namespace ThLaunchSite
{
    /// <summary>
    /// AboutDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();

            AppNameBlock.Text = VersionInfo.AppName;
            VersionBlock.Text = $"Version.{VersionInfo.AppVersion}";
            DeveloperBlock.Text = $"Developer {VersionInfo.Developer}";
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _ = this.Owner.Activate();
        }
    }
}
