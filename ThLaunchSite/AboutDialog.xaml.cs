using System.Text;

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
            DotNetVersionBlock.Text = $"Runtime: {VersionInfo.DotNetViersion}";

            string readmeTextData = GetReadme();
            ReadMeTextBox.Text = readmeTextData;

            string licenseTextData = GetLicense();
            LicenseBox.Text = licenseTextData;
        }

        private string GetReadme()
        {
            string readme = PathInfo.Readme;
            try
            {
                StreamReader fileStream = new(readme, Encoding.UTF8);
                string readmeTextData = fileStream.ReadToEnd();
                fileStream.Close();

                return readmeTextData;
            }
            catch (Exception)
            {
                return "ReadMe.txtを読み込めません。";
            }
        }

        private string GetLicense()
        {
            string license = PathInfo.LicenseFile;
            try
            {
                StreamReader fileStream = new(license, Encoding.UTF8);
                string licenseTextData = fileStream.ReadToEnd();
                fileStream.Close();

                return licenseTextData;
            }
            catch (Exception)
            {
                return "License.txtを読み込めません。";
            }
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _ = this.Owner.Activate();
        }
    }
}
