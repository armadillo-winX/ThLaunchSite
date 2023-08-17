global using System;
global using System.Diagnostics;
global using System.IO;
global using System.Windows;

global using ThLaunchSite.Exceptions;
global using ThLaunchSite.Game;
global using ThLaunchSite.Settings;

using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

namespace ThLaunchSite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? _gameName;
        private DateTime _gameStartTime;

        private string GameId { get; set; }

        private string GameName 
        { 
            get 
            {
                return _gameName;
            }

            set 
            { 
                _gameName = value;
                GameTitleBlock.Text = value;
            } 
        } 

        private string GameProcessName { get; set; }

        private DateTime GameStartTime 
        {
            get
            {
                return _gameStartTime;
            }

            set
            {
                _gameStartTime = value;
                GameStartTimeBlock.Text = $"ゲーム開始 {value:yyyy/MM/dd HH:mm:ss}";
            }
        }

        private AboutDialog? _aboutDialog = null;
        private BackgroundWorker? _gameWaitingWorker = null;
        private DispatcherTimer? _gameControlTimer = null;

        private readonly string? _appName = VersionInfo.AppName;
        private readonly string? _appVersion = VersionInfo.AppVersion;
        private readonly string? _settingsDirectory = PathInfo.SettingsDirectory;
        private readonly string _gamePlayLogFile = PathInfo.GamePlayLogFile;

        private readonly Dictionary<string, int> GameDictionary =
            new()
            {
                { "Th06", 0 },
                { "Th07", 1 },
                { "Th08", 2 },
                { "Th09", 3 },
                { "Th10", 4 },
                { "Th11", 5 },
                { "Th12", 6 },
                { "Th13", 7 },
                { "Th14", 8 },
                { "Th15", 9 },
                { "Th16", 10 },
                { "Th17", 11 },
                { "Th18", 12 }
            };

        private readonly Dictionary<string, int> ThemeDictionary =
            new()
            {
                { "Light", 0 },
                { "Dark" , 1 },
                { "Black", 2 },
                { "NormalColor", 3 }
            };

        public MainWindow()
        {
            InitializeComponent();

            string appProcessName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcessesByName(appProcessName).Length > 1)
            {
                MessageBox.Show(this, $"{_appName}が既に起動しています。\n多重起動はできません。", "多重起動の検出",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            this.Title = _appName;
            if (((App)Application.Current).IsAdmin())
            {
                AuthorityBlock.Content = "管理者権限";
            }
            else
            {
                AuthorityBlock.Content = "ユーザー権限";
            }

            this.GameId = string.Empty;
            this.GameName = string.Empty;
            this.GameProcessName = string.Empty;

            EnableLimitationMode(false);

            if (!Directory.Exists(_settingsDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_settingsDirectory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"設定ファイルディレクトリの生成に失敗。\nアプリケーションを終了します。\n\n[詳細]\n{ex.Message}",
                        "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }

            if (!File.Exists(PathInfo.ExternalToolsConfig))
            {
                try
                {
                    ExternalTool.CreateExternalConfigFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"外部ツール管理ファイルの生成に失敗。\n\n[詳細]\n{ex.Message}",
                        "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            try
            {
                SettingsConfiguration.ConfigureGamePathSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ゲームパス設定の構成に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                ConfigureApplicationSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"アプリケーション設定の構成に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            GetExternalTools();

            ViewGamePlayLogData();

            AppStatusBlock.Content = "準備完了";
        }

        private async void LaunchGame(string gameId, int patchIndex)
        {
            try
            {
                EnableLimitationMode(true);

                string gameProcessName;

                switch (patchIndex)
                {
                    case 0:
                        gameProcessName = await Task.Run(() 
                            => GameProcessHandler.StartGameProcess(gameId)
                            );
                        EnableWaitGameEndMode(gameProcessName);
                        break;
                    case 1:
                        gameProcessName = await Task.Run(() 
                            => GameProcessHandler.StartGameProcessWithPatch(gameId, "vpatch.exe")
                            );
                        EnableWaitGameEndMode(gameProcessName);
                        break;
                    case 2:
                        string[] thpracFiles = GameFile.GetThpracFiles(gameId);
                        if (thpracFiles.Length == 1)
                        {
                            gameProcessName = await Task.Run(()
                                => GameProcessHandler.StartGameProcessWithPatch(gameId, Path.GetFileName(thpracFiles[0]))
                            );
                            EnableWaitGameEndMode(gameProcessName);
                            break;
                        }
                        else if (thpracFiles.Length > 1)
                        {
                            SelectThpracDialog thpracDialog = new()
                            {
                                ThpracFiles = thpracFiles,
                                Owner = this
                            };

                            if (thpracDialog.ShowDialog() == true)
                            {
                                gameProcessName = await Task.Run(()
                                    => GameProcessHandler.StartGameProcessWithPatch(gameId, thpracDialog.ThpracFileName)
                                );
                                EnableWaitGameEndMode(gameProcessName);
                                break;
                            }
                            else
                            {
                                EnableLimitationMode(false);
                                break;
                            }
                        }
                        else
                        {
                            _ = MessageBox.Show(this, "thpracの実行ファイルが見つかりませんでした。", "エラー",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            EnableLimitationMode(false);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ゲームの起動に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                EnableLimitationMode(false);
            }
        }

        private void ViewGamePlayLogData()
        {
            if (File.Exists(_gamePlayLogFile))
            {
                try
                {
                    ObservableCollection<GamePlayLogData> gamePlayLogDatas = new();
                    gamePlayLogDatas = GamePlayLogRecorder.GetGamePlayLogDatas();
                    GameLogDataGrid.AutoGenerateColumns = false;
                    GameLogDataGrid.DataContext = gamePlayLogDatas;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"ゲーム実行履歴の取得に失敗。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ConfigureApplicationSettings()
        {
            ApplicationSettings applicationSettings = SettingsConfiguration.ConfigureApplicationSettings();
            this.Width = applicationSettings.MainWindowWidth;
            this.Height = applicationSettings.MainWindowHeight;
            this.Topmost = applicationSettings.AlwaysOnTop;
            AlwaysOnTopCheckBox.IsChecked = applicationSettings.AlwaysOnTop;
            ResizeRateComboBox.SelectedIndex = applicationSettings.ResizeRateIndex;
            if (applicationSettings.ResizeByRate) ResizeByRateRadioButton.IsChecked = true;
            if (applicationSettings.ResizeBySize) ResizeBySizeRadioButton.IsChecked = true;

            ResizeRateComboBox.IsEnabled = applicationSettings.ResizeByRate;
            GameWindowWidthBox.IsEnabled = applicationSettings.ResizeBySize;
            GameWindowHeightBox.IsEnabled = applicationSettings.ResizeBySize;
            GameWindowWidthBox.Text = applicationSettings.ResizeWidth;
            GameWindowHeightBox.Text = applicationSettings.ResizeHeight;

            string? selectedGameId = applicationSettings.SelectedGameId;
            if (!string.IsNullOrEmpty(selectedGameId))
            {
                GameComboBox.SelectedIndex = GameDictionary[selectedGameId];
            }
            else
            {
                GameComboBox.SelectedIndex = 0;
            }

            string? themeName = applicationSettings.ThemeName;
            if (!string.IsNullOrEmpty(themeName))
            {
                ThemeSettingsComboBox.SelectedIndex = ThemeDictionary[themeName];
            }
            else
            {
                ThemeSettingsComboBox.SelectedIndex = 0;
            }

            if (applicationSettings.FixMainWindowSize)
            {
                this.ResizeMode = ResizeMode.CanMinimize;
                FixMainWindowSizeCheckBox.IsChecked = true;
            }
            else
            {
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
            }
        }

        private void SaveApplicationSettings()
        {
            ApplicationSettings applicationSettings = new()
            {
                MainWindowWidth = this.Width,
                MainWindowHeight = this.Height,
                FixMainWindowSize = FixMainWindowSizeCheckBox.IsChecked == true,
                SelectedGameId = this.GameId,
                AlwaysOnTop = AlwaysOnTopCheckBox.IsChecked == true,
                ResizeRateIndex = ResizeRateComboBox.SelectedIndex,
                ResizeByRate = ResizeByRateRadioButton.IsChecked == true,
                ResizeBySize = ResizeBySizeRadioButton.IsChecked == true,
                ResizeWidth = GameWindowWidthBox.Text,
                ResizeHeight = GameWindowHeightBox.Text,
                ThemeName = ApplicationTheme.ThemeName
            };

            SettingsConfiguration.SaveApplicationSettings(applicationSettings);
        }

        private void GetExternalTools()
        {
            ExternalToolsMenuItem.Items.Clear();

            string exToolsConfig = PathInfo.ExternalToolsConfig;
            if (File.Exists(exToolsConfig))
            {
                try
                {
                    XmlDocument exToolsConfigXml = new();
                    exToolsConfigXml.Load(exToolsConfig);
                    XmlNodeList exToolsNodeList = exToolsConfigXml.SelectNodes("ExternalTools/ExternalTool");
                    if (exToolsNodeList.Count > 0)
                    {
                        foreach (XmlNode toolNode in exToolsNodeList)
                        {
                            string toolName = toolNode.SelectSingleNode("Name").InnerText;

                            MenuItem item = new()
                            {
                                Header = toolName
                            };
                            item.Click += new RoutedEventHandler(LaunchExternalToolMenuItemClick);
                            ExternalToolsMenuItem.Items.Add(item);
                        }
                    }
                    else
                    {
                        MenuItem item = new()
                        {
                            Header = "(なし)",
                            IsEnabled = false
                        };
                        ExternalToolsMenuItem.Items.Add(item);
                    }
                }
                catch (Exception)
                {
                    MenuItem item = new()
                    {
                        Header = "(なし)",
                        IsEnabled = false
                    };
                    ExternalToolsMenuItem.Items.Add(item);
                }
            }
            else
            {
                MenuItem item = new()
                {
                    Header = "(なし)",
                    IsEnabled = false
                };
                ExternalToolsMenuItem.Items.Add(item);
            }
        }

        private void LaunchExternalToolMenuItemClick(object sender, RoutedEventArgs e)
        {
            //MenuItemのHeaderプロパティを取得
            string name = ((MenuItem)sender).Header.ToString();
            try
            {
                ExternalTool.StartExernalToolProcess(name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnableLimitationMode(bool enabled)
        {
            GameComboBox.IsEnabled = !enabled;
            LaunchGameMenuItem.IsEnabled = !enabled;
            LaunchWithVpatchMenuItem.IsEnabled = !enabled;
            LaunchWithThpracMenuItem.IsEnabled = !enabled;
            LaunchCustomProgramMenuItem.IsEnabled = !enabled;
            CatchGameProcessMenuItem.IsEnabled = !enabled;
            CommandGameLauncherMenuItem.IsEnabled = !enabled;

            GamePathBox.IsEnabled = !enabled;
            GamePathBrowseButton.IsEnabled = !enabled;
            LaunchGameButton.IsEnabled = !enabled;
            CatchGameProcessButton.IsEnabled = !enabled;
            CommandGameLauncherButton.IsEnabled = !enabled;

            KillGameProcessMenuItem.IsEnabled = enabled;
            KillGameButton.IsEnabled = enabled;
            ResizeButton.IsEnabled = enabled;
        }

        private void EnableWaitGameEndMode(string gameProcessName)
        {
            this.GameProcessName= gameProcessName;
            this.GameStartTime= DateTime.Now;

            AppStatusBlock.Content = "ゲームの終了を待機中";
            EnableLimitationMode(true);

            _gameWaitingWorker = new BackgroundWorker();
            _gameWaitingWorker.DoWork += new DoWorkEventHandler(WorkerDowork);
            _gameWaitingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerRunningComplete);
            _gameWaitingWorker.RunWorkerAsync(gameProcessName);

            GameTitleBlock.Text = this.GameName;

            _gameControlTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _gameControlTimer.Tick += (e, s) =>
            {
                TimeSpan time = DateTime.Now - this.GameStartTime;

                string gameRunningTime = time.ToString(@"mm\m\i\nss\s\e\c");
                GameRunningTimeBlock.Text = gameRunningTime;
            };
            _gameControlTimer.Start();
        }

        private void ShowCommandGameLauncherDialog()
        {
            if (_gameWaitingWorker != null && _gameWaitingWorker.IsBusy)
            {
                MessageBox.Show(this, "ゲーム終了待機中は他のゲームを起動することはできません。", _appName,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                CommandDialog commandDialog = new()
                {
                    Owner = this
                };
                if (commandDialog.ShowDialog() == true)
                {
                    string? gameId = commandDialog.GameId;
                    int patchIndex = commandDialog.PatchIndex;
                    if (!string.IsNullOrEmpty(gameId))
                    {
                        if (GameDictionary.ContainsKey(gameId))
                        {
                            int index = GameDictionary[gameId];
                            GameComboBox.SelectedIndex = index;
                            LaunchGame(gameId, patchIndex);
                        }
                        else
                        {
                            MessageBox.Show(this, "未対応のゲーム作品です。", "エラー",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void WorkerDowork(object sender, DoWorkEventArgs e)
        {
            string gameProcessName = (string)e.Argument;

            while (GameProcessHandler.IsRunningGame(gameProcessName))
            {
                Thread.Sleep(500);
            }
        }

        private void WorkerRunningComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            _gameControlTimer.Stop();

            GamePlayLogData gamePlayLogData = new()
            {
                GameId = this.GameId,
                GameName = this.GameName,
                GameStartTime = this.GameStartTime.ToString("yyyy/MM/dd HH:mm:ss")
            };

            DateTime gameEndTime = DateTime.Now;
            gamePlayLogData.GameEndTime = gameEndTime.ToString("yyyy/MM/dd HH:mm:ss");

            TimeSpan runningTimeSpan = gameEndTime - this.GameStartTime;
            gamePlayLogData.GameRunningTime = runningTimeSpan.ToString("mm\\:ss");

            try
            {
                GamePlayLogRecorder.SaveGamePlayLog(gamePlayLogData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ゲーム実行ログの保存に失敗。\n{ex.Message}", "エラー",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                string gameOperationLogData = GameFile.GetGameOperationLogData(this.GameId);
                GameOperationLogDataBox.Text = gameOperationLogData;
            }
            catch (Exception ex)
            {
                GameOperationLogDataBox.Text = $"東方動作記録の取得に失敗\n{ex.Message}";
            }

            AppStatusBlock.Content = "準備完了";

            this.GameProcessName = string.Empty;
            EnableLimitationMode(false);
            ViewGamePlayLogData();
        }

        private void AboutMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (_aboutDialog == null || !_aboutDialog.IsLoaded)
            {
                _aboutDialog = new()
                {
                    Owner = this
                };
                _aboutDialog.Show();
            }
            else
            {
                _aboutDialog.WindowState = WindowState.Normal;
                _ = _aboutDialog.Activate();
            }
        }

        private void GamePathBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            string fileFilter;
            if (this.GameId == GameIndex.Th06)
            {
                fileFilter = "東方紅魔郷実行ファイル|東方紅魔郷.exe;th06*.exe|すべてのファイル|*.*";
            }
            else
            {
                fileFilter = $"{this.GameName}実行ファイル|{this.GameId.ToLower()}*.exe|すべてのファイル|*.*";
            }

            OpenFileDialog openFileDialog = new()
            {
                Filter = fileFilter
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string gamePath = openFileDialog.FileName;
                GamePathBox.Text = gamePath;
                string gameId = this.GameId;
                GameFile.SetGameFilePath(gameId, gamePath);
            }
        }

        private void LaunchGameMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            LaunchGame(gameId, 0);
        }

        private void LaunchWithVpatchMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            LaunchGame(gameId, 1);
        }

        private void LaunchWithThpracMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            LaunchGame(gameId, 2);
        }

        private void LaunchCustomeMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = this.GameId;
                GameProcessHandler.StartCustomProgramProcess(gameId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"環境カスタムプログラムの起動に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenGameDirectoryMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = this.GameId;
                GameProcessHandler.OpenGameDirectory(gameId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void KillGameProcessMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameProcessName = this.GameProcessName;
                GameProcessHandler.KillGameProcess(gameProcessName);
                MessageBox.Show(this, "ゲームを強制終了させました。", "ゲームプロセスの強制終了",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ゲームの強制終了に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CatchGameProcessMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = GameProcessHandler.SearchRunningGameProcess();
                string gameProcessName = Path.GetFileNameWithoutExtension(GameFile.GetGameFilePath(gameId));
                GameComboBox.SelectedIndex = GameDictionary[gameId];
                EnableWaitGameEndMode(gameProcessName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (_gameWaitingWorker != null && _gameWaitingWorker.IsBusy)
            {
                MessageBoxResult result = MessageBox.Show(
                    this, $"ゲーム終了待機モードです。\n本当に{_appName}を終了させますか。", _appName,
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        SettingsConfiguration.SaveGamePathSettings();
                        SaveApplicationSettings();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"設定の保存に失敗\n{ex.Message}", "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                try
                {
                    SettingsConfiguration.SaveGamePathSettings();
                    SaveApplicationSettings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"設定の保存に失敗\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GameComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GamePathBox.Clear();

            ComboBoxItem item = (ComboBoxItem)GameComboBox.SelectedItem;
            this.GameId = (string)item.Uid;
            this.GameName = GameIndex.GetGameName(this.GameId);
            string? gamePath = GameFile.GetGameFilePath(this.GameId);

            if (!string.IsNullOrEmpty(gamePath))
            {
                GamePathBox.Text = gamePath;
            }
        }

        private void ResizeByRateRadioButtonClick(object sender, RoutedEventArgs e)
        {
            ResizeRateComboBox.IsEnabled = ResizeByRateRadioButton.IsChecked == true;
            GameWindowWidthBox.IsEnabled = ResizeBySizeRadioButton.IsChecked == true;
            GameWindowHeightBox.IsEnabled = ResizeBySizeRadioButton.IsChecked == true;
        }

        private void ResizeBySizeRadioButtonClick(object sender, RoutedEventArgs e)
        {
            ResizeRateComboBox.IsEnabled = ResizeByRateRadioButton.IsChecked == true;
            GameWindowWidthBox.IsEnabled = ResizeBySizeRadioButton.IsChecked == true;
            GameWindowHeightBox.IsEnabled = ResizeBySizeRadioButton.IsChecked == true;
        }

        private void ResizeButtonClick(object sender, RoutedEventArgs e)
        {
            string gameProcessName = this.GameProcessName;

            if (ResizeByRateRadioButton.IsChecked == true)
            {
                if (ResizeRateComboBox.SelectedIndex > -1)
                {
                    ComboBoxItem item = (ComboBoxItem)ResizeRateComboBox.SelectedItem;
                    string rate = (string)item.Content;

                    double resizeRate = double.Parse(rate.Replace("%", "")) / 100;

                    try
                    {
                        int[] gameWindowSizes = GameWindowHandler.GetWindowSizes(gameProcessName);

                        //リサイズ率に基づいてウィンドウの変更サイズを決定
                        int resizeWidth = (int)Math.Round(gameWindowSizes[0] * resizeRate);
                        int resizeHeight = (int)Math.Round(gameWindowSizes[1] * resizeRate);

                        GameWindowHandler.ResizeWindow(gameProcessName, resizeWidth, resizeHeight);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (ResizeBySizeRadioButton.IsChecked == true)
            {
                try
                {
                    int resizeWidth = int.Parse(GameWindowWidthBox.Text);
                    int resizeHeight = int.Parse(GameWindowHeightBox.Text);

                    GameWindowHandler.ResizeWindow(gameProcessName, resizeWidth, resizeHeight);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.P)
            {
                ShowCommandGameLauncherDialog();
            }
        }

        private void LaunchGameButtonClick(object sender, RoutedEventArgs e)
        {
            if (!LaunchGameButtonMenu.IsOpen)
            {
                LaunchGameButtonMenu.PlacementTarget = LaunchGameButton;
                LaunchGameButtonMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                LaunchGameButtonMenu.IsOpen = true;
            }
            else
            {
                LaunchGameButtonMenu.IsOpen = false;
            }
        }

        private void CommandGameLauncherMenuItemClick(object sender, RoutedEventArgs e)
        {
            ShowCommandGameLauncherDialog();
        }

        private void TotalGameRunningTimeStatisticsMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (GameLogDataGrid.Items.Count > 0)
            {
                ObservableCollection<GamePlayLogData> gamePlayLogDatas 
                    = GameLogDataGrid.DataContext as ObservableCollection<GamePlayLogData>;

                int totalGameRunningTime = 0;
                foreach (GamePlayLogData gamePlayLogData in  gamePlayLogDatas)
                {
                    try
                    {
                        string[] gameRunningTimeRecord = gamePlayLogData.GameRunningTime.Split(":");
                        int gameRunningTimeMin = int.Parse(gameRunningTimeRecord[0]) * 60;
                        int gameRunningTimeSec = int.Parse(gameRunningTimeRecord[1]);
                        totalGameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                    }
                    catch (Exception)
                    {
                        
                    }
                }

                int minutes = totalGameRunningTime / 60;
                int seconds = totalGameRunningTime % 60;
                GameRunningTimeStaticsDialog gameRunningTimeStaticsDialog = new()
                {
                    TotalGameRunningTime = $"{minutes}min {seconds}sec",
                    Owner = this
                };
                gameRunningTimeStaticsDialog.ShowDialog();
            }
        }

        private void ManageEnternalToolsMenuItemClick(object sender, RoutedEventArgs e)
        {
            ManageEnternalToolsDialog manageEnternalToolsDialog = new()
            {
                Owner = this
            };

            manageEnternalToolsDialog.ShowDialog();
            GetExternalTools();
        }

        private void AlwaysOnTopCheckBoxClick(object sender, RoutedEventArgs e)
        {
            this.Topmost = AlwaysOnTopCheckBox.IsChecked == true;
        }

        private void ThemeSettingsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)ThemeSettingsComboBox.SelectedItem;
            string themeName = item.Uid;

            ApplicationTheme.ThemeName = themeName;
        }

        private void FixMainWindowSizeCheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (FixMainWindowSizeCheckBox.IsChecked == true)
            {
                this.ResizeMode = ResizeMode.CanMinimize;
            }
            else
            {
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
            }
        }
    }
}
