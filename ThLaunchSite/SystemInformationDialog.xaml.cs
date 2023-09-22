namespace ThLaunchSite
{
    /// <summary>
    /// SystemInformationDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemInformationDialog : Window
    {
        public SystemInformationDialog()
        {
            InitializeComponent();

            OperatingSystemBlock.Text = VersionInfo.OperatingSystem;
            DotNetRuntimeBlock.Text = VersionInfo.DotNetRuntime;
            SystemArchitectureBlock.Text = $"システムのアーキテクチャ: {VersionInfo.SystemArchitecture}";
            ProcessArchitectureBlock.Text = $"プロセスのアーキテクチャ: {VersionInfo.AppArchitecture}";
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
