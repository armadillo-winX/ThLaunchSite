using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThLaunchSite
{
    /// <summary>
    /// CreateAllGamesScoreBackupDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class CreateAllGamesScoreBackupDialog : Window
    {
        public CreateAllGamesScoreBackupDialog()
        {
            InitializeComponent();
        }

        private void CreateScoreBackup()
        {
            List<string> availableGames = GameIndex.GetAvailableGamesList();

            this.Dispatcher.Invoke((Action)(() =>
                MainProgressBar.Maximum = availableGames.Count
            ));

            foreach (string gameId in availableGames)
            {
                try
                {
                    bool result = GameScoreBackup.CreateScoreBackup(gameId);
                    if (result) 
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                            OutputBox.Text += $"{Properties.Resources.MessageCreated} : {GameIndex.GetGameName(gameId)}\n"
                        ));
                    }
                    else
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                            OutputBox.Text += $"{Properties.Resources.MessageScoreFileNotFound} : {GameIndex.GetGameName(gameId)}\n"
                        ));
                    }
                }
                catch (Exception)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                        OutputBox.Text += $"{Properties.Resources.ErrorMessageFailedToCreateScoreBackup} : {GameIndex.GetGameName(gameId)}\n"
                    ));
                }

                this.Dispatcher.Invoke((Action)(() =>
                    MainProgressBar.Value += 1
                ));
            }
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => CreateScoreBackup());
            MessageBox.Show(this,
                Properties.Resources.MessageCreated, Properties.Resources.TitleCreateScoreBackup,
                MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }
    }
}
