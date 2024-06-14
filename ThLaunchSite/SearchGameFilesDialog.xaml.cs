using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThLaunchSite
{
    /// <summary>
    /// SearchGameFilesDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchGameFilesDialog : Window
    {
        private readonly string[] gameFilesName =
            [
                "東方紅魔郷.exe",
                "th07.exe",
                "th08.exe",
                "th08tr.exe",
                "th09.exe",
                "th09tr.exe",
                "th095.exe",
                "th10.exe",
                "th10tr.exe",
                "th11.exe",
                "th12.exe",
                "th125.exe",
                "th128.exe",
                "th13.exe",
                "th14.exe",
                "th143.exe",
                "th15.exe",
                "th16.exe",
                "th165.exe",
                "th17.exe",
                "th18.exe",
                "th185.exe",
                "th19.exe"
            ];

        public SearchGameFilesDialog()
        {
            InitializeComponent();
        }

        private void SearchGameFiles(string rootDirectory)
        {
            IEnumerable<string> directories = Directory.EnumerateDirectories(rootDirectory, "*", 
                new EnumerationOptions() { IgnoreInaccessible = true, RecurseSubdirectories = true });

            foreach (string directory in directories)
            {
                try
                {
                    foreach (string gameFileName in gameFilesName)
                    {
                        string[] gameFiles = Directory.GetFiles(directory, gameFileName);
                        foreach (string gameFile in gameFiles)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                                GameFileListBox.Items.Add(gameFile)
                            ));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new();
            if (openFolderDialog.ShowDialog() == true)
            {
                SearchRootDirectoryBox.Text = openFolderDialog.FolderName;
            }
        }

        private async void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            if (SearchRootDirectoryBox.Text.Length > 0 && Directory.Exists(SearchRootDirectoryBox.Text))
            {
                StatusBlock.Text = Properties.Resources.MessageSearching;
                SearchRootDirectoryBox.IsEnabled = false;
                SearchButton.IsEnabled = false;
                BrowseButton.IsEnabled = false;
                SetPathButton.IsEnabled = false;

                GameFileListBox.Items.Clear();

                string rootDirectory = SearchRootDirectoryBox.Text;

                await Task.Run(() => SearchGameFiles(rootDirectory));

                StatusBlock.Text = "";
                SearchRootDirectoryBox.IsEnabled = true;
                SearchButton.IsEnabled = true;
                BrowseButton.IsEnabled = true;
                SetPathButton.IsEnabled = true;
            }
            else if (!Directory.Exists(SearchRootDirectoryBox.Text))
            {
                MessageBox.Show(this, Properties.Resources.MessageSearchRootFolderNotFound, Properties.Resources.TitleSearchGameFile,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (SearchRootDirectoryBox.Text.Length == 0)
            {
                MessageBox.Show(this, Properties.Resources.MessageSetSearchRootFolder, Properties.Resources.TitleSearchGameFile,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void SetPathButtonClick(object sender, RoutedEventArgs e)
        {
            if (GameFileListBox.SelectedIndex > -1)
            {
                string gameFilePath = GameFileListBox.SelectedItem.ToString();

                string gameId;
                if (Path.GetFileNameWithoutExtension(gameFilePath) == "東方紅魔郷")
                {
                    gameId = "Th06";
                }
                else
                {
                    gameId = Path.GetFileNameWithoutExtension(gameFilePath).Replace("tr", "").Replace("t", "T");
                }

                GameFile.SetGameFilePath(gameId, gameFilePath);
                MessageBox.Show(this,
                    $"{GameIndex.GetGameName(gameId)}{Properties.Resources.MessageGamePathRegisterToSettings}",
                    Properties.Resources.TitleSearchGameFile,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(this, Properties.Resources.MessageSelectPathFromGameFilesList, Properties.Resources.TitleSearchGameFile,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
