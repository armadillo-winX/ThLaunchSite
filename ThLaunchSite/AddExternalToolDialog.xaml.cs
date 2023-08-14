using Microsoft.Win32;

namespace ThLaunchSite
{
    /// <summary>
    /// AddExternalToolDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AddExternalToolDialog : Window
    {
        public AddExternalToolDialog()
        {
            InitializeComponent();

            _ = PathBox.Focus();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "実行ファイル|*.exe;*.bat;*.cmd;*.vbs",
                FileName = ""
            };
            if (openFileDialog.ShowDialog(this) == true)
            {
                string toolPath = openFileDialog.FileName;
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(toolPath);
                string productName = fileVersionInfo.ProductName;
                if (ToolNameBox.Text.Length == 0 &&
                    !string.IsNullOrEmpty(productName))
                    ToolNameBox.Text = productName;
                PathBox.Text = toolPath;
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string toolName = ToolNameBox.Text;
                string toolPath = PathBox.Text;
                string option = OptionBox.Text;
                bool asAdmin = AsAdminCheckBox.IsChecked == true;
                if (toolName.Length > 0 && toolPath.Length > 0)
                {
                    ExternalTool.AddExternalTool(toolName, toolPath, option, asAdmin);
                    this.DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
