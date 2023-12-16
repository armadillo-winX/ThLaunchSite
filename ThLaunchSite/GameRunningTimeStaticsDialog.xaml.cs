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

        //ToDo : もう少し良い実装を考える
        private void ViewGameRunningTimeStatics(object datas)
        {
            ObservableCollection<GamePlayLogData> gamePlayLogDatas
                    = datas as ObservableCollection<GamePlayLogData>;

            int totalGameRunningTime = 0;
            int th06GameRunningTime = 0;
            int th07GameRunningTime = 0;
            int th08GameRunningTime = 0;
            int th09GameRunningTime = 0;
            int th10GameRunningTime = 0;
            int th11GameRunningTime = 0;
            int th12GameRunningTime = 0;
            int th13GameRunningTime = 0;
            int th14GameRunningTime = 0;
            int th15GameRunningTime = 0;
            int th16GameRunningTime = 0;
            int th17GameRunningTime = 0;
            int th18GameRunningTime = 0;
            foreach (GamePlayLogData gamePlayLogData in gamePlayLogDatas)
            {
                try
                {
                    string gameId = gamePlayLogData.GameId;

                    string[] gameRunningTimeRecord = gamePlayLogData.GameRunningTime.Split(":");
                    int gameRunningTimeMin = int.Parse(gameRunningTimeRecord[0]) * 60;
                    int gameRunningTimeSec = int.Parse(gameRunningTimeRecord[1]);
                    totalGameRunningTime += gameRunningTimeMin + gameRunningTimeSec;

                    if (gameId == GameIndex.Th06)
                    {
                        th06GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th07)
                    {
                        th07GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th08)
                    {
                        th08GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th09)
                    {
                        th09GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th10)
                    {
                        th10GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th11)
                    {
                        th11GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th12)
                    {
                        th12GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th13)
                    {
                        th13GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th14)
                    {
                        th14GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th15)
                    {
                        th15GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th16)
                    {
                        th16GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th17)
                    {
                        th17GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    else if (gameId == GameIndex.Th18)
                    {
                        th18GameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                }
                catch (Exception)
                {

                }
            }

            int totalMinutes = totalGameRunningTime / 60;
            int totalSeconds = totalGameRunningTime % 60;

            GameRunningTimeBlock.Text = $"{totalMinutes:00}min {totalSeconds:00}sec";

            ObservableCollection<GamePlayLogData> eachGameRunningTimeDatas = new() 
            { 
                new GamePlayLogData 
                {
                    GameId = GameIndex.Th06, 
                    GameName = GameIndex.GetGameName(GameIndex.Th06), 
                    GameRunningTime = $"{th06GameRunningTime / 60:00}:{th06GameRunningTime % 60:00}"
                },
                new GamePlayLogData 
                {
                    GameId = GameIndex.Th07,
                    GameName = GameIndex.GetGameName(GameIndex.Th07),
                    GameRunningTime = $"{th07GameRunningTime / 60:00}:{th07GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th08,
                    GameName = GameIndex.GetGameName(GameIndex.Th08),
                    GameRunningTime = $"{th08GameRunningTime / 60:00}:{th08GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th09,
                    GameName = GameIndex.GetGameName(GameIndex.Th09),
                    GameRunningTime = $"{th09GameRunningTime / 60:00}:{th09GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th10,
                    GameName = GameIndex.GetGameName(GameIndex.Th10),
                    GameRunningTime = $"{th10GameRunningTime / 60:00}:{th10GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th11,
                    GameName = GameIndex.GetGameName(GameIndex.Th11),
                    GameRunningTime = $"{th11GameRunningTime / 60:00}:{th11GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th12,
                    GameName = GameIndex.GetGameName(GameIndex.Th12),
                    GameRunningTime = $"{th12GameRunningTime / 60:00}:{th12GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th13,
                    GameName = GameIndex.GetGameName(GameIndex.Th13),
                    GameRunningTime = $"{th13GameRunningTime / 60:00}:{th13GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th14,
                    GameName = GameIndex.GetGameName(GameIndex.Th14),
                    GameRunningTime = $"{th14GameRunningTime / 60:00}:{th14GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th15,
                    GameName = GameIndex.GetGameName(GameIndex.Th15),
                    GameRunningTime = $"{th15GameRunningTime / 60:00}:{th15GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th16,
                    GameName = GameIndex.GetGameName(GameIndex.Th16),
                    GameRunningTime = $"{th16GameRunningTime / 60:00}:{th16GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th17,
                    GameName = GameIndex.GetGameName(GameIndex.Th17),
                    GameRunningTime = $"{th17GameRunningTime / 60:00}:{th17GameRunningTime % 60:00}"
                },
                new GamePlayLogData
                {
                    GameId = GameIndex.Th18,
                    GameName = GameIndex.GetGameName(GameIndex.Th18),
                    GameRunningTime = $"{th18GameRunningTime / 60:00}:{th18GameRunningTime % 60:00}"
                }
            };
            EachGameRunningTimeGrid.AutoGenerateColumns = false;
            EachGameRunningTimeGrid.DataContext = eachGameRunningTimeDatas;
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
