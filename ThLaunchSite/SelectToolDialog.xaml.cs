namespace ThLaunchSite
{
    /// <summary>
    /// SelectToolDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectToolDialog : Window
    {
        public string GameId
        {
            set
            {
                string gamePath = GameFile.GetGameFilePath(value);
                string gameDirectory = Path.GetDirectoryName(gamePath);

                string[] executableFiles
                    = Directory.GetFiles(gameDirectory, "*.exe", SearchOption.TopDirectoryOnly);

                foreach (string executableFile in executableFiles)
                {
                    string executableFileName = Path.GetFileName(executableFile);
                    string gameFileName = Path.GetFileName(gamePath);
                    if (!string.IsNullOrEmpty(executableFileName) &&
                        executableFileName != gameFileName &&
                        executableFileName != "custom.exe" &&
                        !executableFileName.Contains("update")) 
                    {
                        ToolsListBox.Items.Add(executableFileName);
                    }
                }
            }
        }

        public string? ToolName { get; set; }

        public SelectToolDialog()
        {
            InitializeComponent();
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            if (ToolsListBox.SelectedIndex > -1)
            {
                this.ToolName = ToolsListBox.SelectedItem.ToString();
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(this, "適用するツールを選択してください。", "適用するツールの選択",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
