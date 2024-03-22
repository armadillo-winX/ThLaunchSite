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
                "th10.exe",
                "th10tr.exe",
                "th11.exe",
                "th12.exe",
                "th13.exe",
                "th14.exe",
                "th15.exe",
                "th16.exe",
                "th17.exe",
                "th18.exe",
                "th19.exe"
            ];

        public SearchGameFilesDialog()
        {
            InitializeComponent();
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
                StatusBlock.Text = "検索しています...";
                SearchRootDirectoryBox.IsEnabled = false;
                SearchButton.IsEnabled = false;
                BrowseButton.IsEnabled = false;
                SetPathButton.IsEnabled = false;

                GameFileListBox.Items.Clear();

                string rootDirectory = SearchRootDirectoryBox.Text;

                await Task.Run(() =>
                {
                    IEnumerable<string> directories = GetAllDirectories(rootDirectory);

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
                });

                StatusBlock.Text = "";
                SearchRootDirectoryBox.IsEnabled = true;
                SearchButton.IsEnabled = true;
                BrowseButton.IsEnabled = true;
                SetPathButton.IsEnabled = true;
            }
            else if (!Directory.Exists(SearchRootDirectoryBox.Text))
            {
                MessageBox.Show(this, "検索親フォルダが存在しません。", "ゲーム実行ファイルの検索",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (SearchRootDirectoryBox.Text.Length == 0)
            {
                MessageBox.Show(this, "検索親フォルダを指定してください。", "ゲーム実行ファイルの検索",
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
                    $"'{gameFilePath}' を{GameIndex.GetGameName(gameId)}の実行ファイルとして登録しました。",
                    "ゲーム実行ファイルの検索",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(this, "ゲーム実行ファイル一覧からパスを一つ選択してください。", "ゲーム実行ファイルの検索",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private IEnumerable<string> GetAllDirectories(string rootDirectory)
        {
            Queue<string> directories = new();

            directories.Enqueue(rootDirectory);
            while (directories.Count != 0)
            {
                string? directory = directories.Dequeue();
                if (Directory.Exists(directory))
                {
                    yield return directory;
                    try
                    {
                        IEnumerable<string> childDirectories = Directory.EnumerateDirectories(directory);
                        foreach (string? childDirectory in childDirectories)
                            directories.Enqueue(childDirectory);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
