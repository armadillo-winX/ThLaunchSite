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
            DeveloperBlock.Text = $"by {VersionInfo.Developer}";
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _ = this.Owner.Activate();
        }

        private void OpenSystemInformationDialogClick(object sender, RoutedEventArgs e)
        {
            SystemInformationDialog systemInformationDialog = new()
            {
                Owner = this.Owner
            };

            systemInformationDialog.ShowDialog();
        }
    }
}
