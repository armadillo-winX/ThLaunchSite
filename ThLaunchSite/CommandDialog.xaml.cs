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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string commandHelp = "起動したい東方原作を th<作品番号> で指定してください。\n\n例\n\n東方妖々夢\nth07\n\n東方輝針城\nth14\n\n\n";
            string optionHelp = "また、以下のオプションを付与できます。\n/vp, /vpatch, /vsyncpatch     VsyncPatchの適用\n/tp, /thprac     thpracの適用\n\n\n";
            string exmaple 
                = "<コマンド入力例>\n\n東方紅魔郷を起動する場合\nth06\n\nVsyncPatchを適用して東方永夜抄を起動する場合\nth08 /vpatch\n\nthpracを適用して風神録を起動する場合\nth10 /tp\n\n\n";
            string runHelp = "入力例のようにテキストボックスにコマンドを入力し、Enterキーを押すか、[起動(L)]ボタンをクリックするとゲームが起動します。";

            string helpMessage = commandHelp + optionHelp + exmaple + runHelp;
            MessageBox.Show(this, helpMessage, "コマンドゲームランチャーのヘルプ",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
