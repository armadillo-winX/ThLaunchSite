namespace ThLaunchSite
{
    /// <summary>
    /// SelectThpracDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectThpracDialog : Window
    {
        public string[]? ThpracFiles
        {
            set
            {
                if (value.Length > 0)
                {
                    foreach (string thpracFile in value)
                    {
                        ThpracFilesListBox.Items.Add(Path.GetFileName(thpracFile));
                    }
                }
            } 
        }

        public string? ThpracFileName { get; set; }

        public SelectThpracDialog()
        {
            InitializeComponent();
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            if (ThpracFilesListBox.SelectedIndex > -1)
            {
                this.ThpracFileName = ThpracFilesListBox.SelectedItem as string;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                _ = MessageBox.Show(this, Properties.Resources.MessageSelectThpracFile, Properties.Resources.TitleSelectThprac,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
