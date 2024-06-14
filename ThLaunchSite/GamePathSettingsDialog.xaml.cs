using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ThLaunchSite
{
    /// <summary>
    /// GamePathSettingsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GamePathSettingsDialog : Window
    {
        public GamePathSettingsDialog()
        {
            InitializeComponent();

            List<string> allGamesList = GameIndex.GetAllGamesList();

            foreach (string gameId in allGamesList)
            {
                ListBoxItem item = new()
                {
                    Content = $"{gameId}: {GameIndex.GetGameName(gameId)}",
                    Uid = gameId
                };
                GameListBox.Items.Add(item);
            }

            GameListBox.SelectedIndex = 0;
            GameListBox.Focus();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            if (GameListBox.SelectedIndex > -1)
            {
                string gameId = ((ListBoxItem)GameListBox.SelectedItem).Uid;

                string fileFilter;
                if (gameId == "Th06")
                {
                    fileFilter = $"東方紅魔郷{Properties.Resources.TextExecutableFile}|東方紅魔郷.exe;th06*.exe|{Properties.Resources.TextAllFile}|*.*";
                }
                else
                {
                    fileFilter = $"{GameIndex.GetGameName(gameId)}{Properties.Resources.TextExecutableFile}|{gameId.ToLower()}*.exe|{Properties.Resources.TextAllFile}|*.*";
                }

                OpenFileDialog openFileDialog = new()
                {
                    Filter = fileFilter
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string gamePath = openFileDialog.FileName;
                    GamePathBox.Text = gamePath;
                    GameFile.SetGameFilePath(gameId, gamePath);
                }
            }
            else
            {
                MessageBox.Show(this, Properties.Resources.MessageSelectGameToSetPathFromList, Properties.Resources.TitleSetGamePath,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void GameListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string gameId = ((ListBoxItem)GameListBox.SelectedItem).Uid;

            string gamePath = GameFile.GetGameFilePath(gameId);
            GamePathBox.Text = gamePath;

            string gameEdition = GameIndex.GetGameEdition(gameId);
            if (gameEdition == "Trial")
            {
                TrialCheckBox.IsChecked = true;
            }
            else
            {
                TrialCheckBox.IsChecked = false;
            }
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SearchGameFileButtonClick(object sender, RoutedEventArgs e)
        {
            SearchGameFilesDialog searchGameFilesDialog = new()
            {
                Owner = this
            };
            searchGameFilesDialog.ShowDialog();
            if (GameListBox.SelectedIndex > -1)
            {
                string gameId = ((ListBoxItem)GameListBox.SelectedItem).Uid;

                string gamePath = GameFile.GetGameFilePath(gameId);
                GamePathBox.Text = gamePath;
            }
        }

        private void TrialCheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (GameListBox.SelectedIndex > -1)
            {
                string gameId = ((ListBoxItem)GameListBox.SelectedItem).Uid;
                if (TrialCheckBox.IsChecked == true)
                {
                    GameIndex.SetGameEdition(gameId, "Trial");
                }
                else
                {
                    GameIndex.SetGameEdition(gameId, "Product");
                }
            }
            else
            {
                MessageBox.Show(this, Properties.Resources.MessageSelectGameToSetPathFromList, Properties.Resources.TitleSetGamePath,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                TrialCheckBox.IsChecked = false;
            }
        }
    }
}
