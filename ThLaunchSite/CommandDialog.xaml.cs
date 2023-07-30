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

            _ = CommandBox.Focus();
        }

        private void CommandParser(string command)
        {
            if (!string.IsNullOrEmpty(command))
            {
                string[] commands = command.Replace(" ", "").Split("/");
                string gameId = commands[0].Replace("th", "Th");
                this.GameId = gameId;

                if (commands.Length > 1)
                {
                    string patchOption = commands[1];
                    if (patchOption == "vp" || patchOption == "vpatch" || patchOption == "vsyncpatch")
                    {
                        this.PatchIndex = 1;
                    }
                    else if (patchOption == "tp" || patchOption == "thprac")
                    {
                        this.PatchIndex = 2;
                    }
                    else
                    {
                        this.PatchIndex = 0;
                    }
                }
                else
                {
                    this.PatchIndex = 0;
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string command = CommandBox.Text;
            CommandParser(command);
            DialogResult = true;
            this.Close();
        }
    }
}
