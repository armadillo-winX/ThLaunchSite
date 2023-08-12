namespace ThLaunchSite
{
    /// <summary>
    /// SelectThpracDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectThpracDialog : Window
    {
        public string[]? ThpracFiles { get; set; }

        public string? ThpracFileName { get; set; }

        public SelectThpracDialog()
        {
            InitializeComponent();

            if (this.ThpracFiles != null && this.ThpracFiles.Length > 0)
            {
                foreach (string thpracFile in this.ThpracFiles)
                {
                    ThpracFilesListBox.Items.Add(Path.GetFileName(thpracFile));
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (ThpracFilesListBox.SelectedIndex > -1)
            {
                this.ThpracFileName = ThpracFilesListBox.SelectedItem as string;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                _ = MessageBox.Show(this, "適用するthpracを選択してください。", "thprac選択",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
