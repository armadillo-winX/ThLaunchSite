using System.Windows.Controls;

namespace ThLaunchSite
{
    /// <summary>
    /// CommandDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class CommandDialog : Window
    {
        public string? GameId { get; set; }

        public CommandDialog()
        {
            InitializeComponent();

            _ = CommandBox.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.GameId = CommandBox.Text.Replace("th", "Th");
            DialogResult = true;
            this.Close();
        }
    }
}
