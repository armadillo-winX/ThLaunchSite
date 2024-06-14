using System.Collections.Generic;

namespace ThLaunchSite
{
    /// <summary>
    /// InstallToolsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class InstallToolsDialog : Window
    {
        public InstallToolsDialog()
        {
            InitializeComponent();
        }

        private void InstallTool(string gameId, string toolPath)
        {
            string toolName = Path.GetFileName(toolPath);
            try
            {
                string gameDirectory = Path.GetDirectoryName(GameFile.GetGameFilePath(gameId));

                if (File.Exists($"{gameDirectory}\\{toolName}"))
                {
                    MessageBoxResult result = MessageBox.Show(
                        this, $"{Properties.Resources.LabelInstallTarget}{GameIndex.GetGameName(gameId)}\n{toolName}{Properties.Resources.MessageAskOverwriteTool}",
                        Properties.Resources.TitleInstallTools,
                        MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        File.Copy(toolPath, $"{gameDirectory}\\{toolName}", true);

                        LogBox.Text += $"{Properties.Resources.MessageOverwriteInstall}:{toolName} ({Properties.Resources.LabelInstallTarget}{GameIndex.GetGameName(gameId)})\n";
                    }
                    else
                    {
                        LogBox.Text += $"{toolName}{Properties.Resources.MessageCanceledInstall} ({Properties.Resources.LabelInstallTarget}{GameIndex.GetGameName(gameId)})\n";
                    }
                }
                else
                {
                    File.Copy(toolPath, $"{gameDirectory}\\{toolName}");
                    LogBox.Text += $"{Properties.Resources.MessageInstall}{toolName} ({Properties.Resources.LabelInstallTarget}{GameIndex.GetGameName(gameId)})\n";
                }
            }
            catch (Exception ex)
            {
                LogBox.Text += $"{toolName}{Properties.Resources.ErrorMessageFailedToInstall}:{ex.Message} ({Properties.Resources.LabelInstallTarget}{GameIndex.GetGameName(gameId)})\n";
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowsLoaded(object sender, RoutedEventArgs e)
        {
            List<string> enabledGames = GameIndex.GetAvailableGamesList();
            foreach (string enabledGame in enabledGames)
            {
                GameListBox.Items.Add(GameIndex.GetGameName(enabledGame));
            }
        }

        private void WindowPreviewDragOver(object sender, DragEventArgs e)
        {
            //ファイルがドラッグされたとき、カーソルをドラッグ中のアイコンに変更し、そうでない場合は何もしない。
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void WindowPreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) // ドロップされたものがファイルかどうか確認する。
            {
                string[] toolPaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (GameListBox.SelectedItems.Count > 0)
                {
                    foreach (string gameName in GameListBox.SelectedItems)
                    {
                        string gameId = GameIndex.GetGameIdFromGameName(gameName);
                        foreach (string toolPath in toolPaths)
                        {
                            InstallTool(gameId, toolPath);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, Properties.Resources.MessageNoInstallTargetSelected, Properties.Resources.TitleInstallTools,
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
    }
}
