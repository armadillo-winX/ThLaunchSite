using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThLaunchSite
{
    /// <summary>
    /// GameRunningTimeStaticsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GameRunningTimeStaticsDialog : Window
    {
        public object? GamePlayLogDatas
        {
            set
            {
                ViewGameRunningTimeStatics(value);
            }
        }

        public GameRunningTimeStaticsDialog()
        {
            InitializeComponent();
        }

        private void ViewGameRunningTimeStatics(object datas)
        {
            ObservableCollection<GamePlayLogData> gamePlayLogDatas
                    = datas as ObservableCollection<GamePlayLogData>;

            int totalGameRunningTime = 0;

            List<string> allGamesList = GameIndex.GetAllGamesList();

            Dictionary<string, int> gameRunningTimeDictionary = [];

            foreach (string gameId in allGamesList)
            {
                gameRunningTimeDictionary.Add(gameId, 0);
            }

            foreach (GamePlayLogData gamePlayLogData in gamePlayLogDatas)
            {
                try
                {
                    string gameId = gamePlayLogData.GameId;

                    string[] gameRunningTimeRecord = gamePlayLogData.GameRunningTime.Split(":");
                    int gameRunningTimeMin = int.Parse(gameRunningTimeRecord[0]) * 60;
                    int gameRunningTimeSec = int.Parse(gameRunningTimeRecord[1]);
                    totalGameRunningTime += gameRunningTimeMin + gameRunningTimeSec;

                    gameRunningTimeDictionary[gameId] += gameRunningTimeMin + gameRunningTimeSec;
                }
                catch (Exception)
                {

                }
            }

            int totalMinutes = totalGameRunningTime / 60;
            int totalSeconds = totalGameRunningTime % 60;

            GameRunningTimeBlock.Text = $"{totalMinutes:00}min {totalSeconds:00}sec";

            ObservableCollection<GamePlayLogData> eachGameRunningTimeDatas = [];

            foreach (string gameId in allGamesList)
            {
                int gameRunningTime = gameRunningTimeDictionary[gameId];

                GamePlayLogData gamePlayLogData = new()
                {
                    GameId = gameId,
                    GameName = GameIndex.GetGameName(gameId),
                    GameRunningTime = $"{gameRunningTime / 60:00}:{gameRunningTime % 60:00}"
                };

                eachGameRunningTimeDatas.Add(gamePlayLogData);
            }

            EachGameRunningTimeGrid.AutoGenerateColumns = false;
            EachGameRunningTimeGrid.DataContext = eachGameRunningTimeDatas;
        }
    }
}
