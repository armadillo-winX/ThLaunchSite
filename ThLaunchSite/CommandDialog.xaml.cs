namespace ThLaunchSite
{
    /// <summary>
    /// CommandDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class CommandDialog : Window
    {
        public string? GameId { get; set; }

        public int PatchIndex { get; set; }

        public CommandDialog()
        {
            InitializeComponent();

            PatchOptionComboBox.SelectedIndex = 0;
            _ = CommandBox.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.GameId = CommandBox.Text.Replace("th", "Th");
            //念のため
            PatchIndex = PatchOptionComboBox.SelectedIndex >= 0 ? PatchOptionComboBox.SelectedIndex : 0;

            DialogResult = true;
            this.Close();
        }
    }
}
