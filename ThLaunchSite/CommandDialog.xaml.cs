﻿namespace ThLaunchSite
{
    /// <summary>
    /// CommandDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class CommandDialog : Window
    {
        public string? GameId { get; set; }

        public int ToolIndex { get; set; }

        public CommandDialog()
        {
            InitializeComponent();

            _ = CommandBox.Focus();
        }

        private void ParseCommand(string command)
        {
            if (!string.IsNullOrEmpty(command))
            {
                string[] commands = command.Replace(" ", "").Split("/");
                string gameId = commands[0].Replace("th", "Th");
                this.GameId = gameId;

                if (commands.Length > 1)
                {
                    string toolOption = commands[1];
                    if (toolOption == "vp" || toolOption == "vpatch" || toolOption == "vsyncpatch")
                    {
                        this.ToolIndex = 1;
                    }
                    else if (toolOption == "tp" || toolOption == "thprac")
                    {
                        this.ToolIndex = 2;
                    }
                    else
                    {
                        this.ToolIndex = 0;
                    }
                }
                else
                {
                    this.ToolIndex = 0;
                }
            }
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            string command = CommandBox.Text;
            ParseCommand(command);
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HelpButtonClick(object sender, RoutedEventArgs e)
        {
            string commandHelp = "起動したい東方原作を th<作品番号> で指定してください。\n\n例\n\n東方妖々夢\nth07\n\n東方輝針城\nth14\n\n\n";
            string optionHelp = "また、以下のオプションを付与できます。\n/vp, /vpatch, /vsyncpatch     VsyncPatchの適用\n/tp, /thprac     thpracの適用\n\n\n";
            string example 
                = "<コマンド入力例>\n\n東方紅魔郷を起動する場合\nth06\n\nVsyncPatchを適用して東方永夜抄を起動する場合\nth08 /vpatch\n\nthpracを適用して東方風神録を起動する場合\nth10 /tp\n\n\n";
            string runHelp = "入力例のようにテキストボックスにコマンドを入力し、Enterキーを押すか、[起動(L)]ボタンをクリックするとゲームが起動します。";

            string helpMessage = commandHelp + optionHelp + example + runHelp;
            MessageBox.Show(this, helpMessage, "コマンドゲームランチャーのヘルプ",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
