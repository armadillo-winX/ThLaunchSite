﻿global using System;
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
using Forms = System.Windows.Forms;

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

        private int GameProcessPriorityIndex { get; set; }

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

        private bool IsEnabledWaitGameEndMode { get; set; }

        private AboutDialog? _aboutDialog = null;
        private BackgroundWorker? _gameWaitingWorker = null;
        private DispatcherTimer? _gameControlTimer = null;

        private readonly string? _appName = VersionInfo.AppName;
        private readonly string? _appVersion = VersionInfo.AppVersion;
        private readonly string? _settingsDirectory = PathInfo.SettingsDirectory;
        private readonly string _gamePlayLogFile = PathInfo.GamePlayLogFile;

        private readonly Dictionary<string, int> _gameDictionary =
            new()
            {
                { "Th06", 0 },
                { "Th07", 1 },
                { "Th08", 2 },
                { "Th09", 3 },
                { "Th095", 4 },
                { "Th10", 5 },
                { "Th11", 6 },
                { "Th12", 7 },
                { "Th125", 8 },
                { "Th128", 9 },
                { "Th13", 10 },
                { "Th14", 11 },
                { "Th143", 12 },
                { "Th15", 13 },
                { "Th16", 14 },
                { "Th165", 15 },
                { "Th17", 16 },
                { "Th18", 17 },
                { "Th185", 18 },
                { "Th19", 19 }
            };

        private readonly Dictionary<string, int> _themeDictionary =
            new()
            {
                { "Light", 0 },
                { "Dark" , 1 },
                { "Black", 2 },
                { "NormalColor", 3 }
            };

        private readonly Dictionary<string, int> _captureFileFormatDictionary
            = new()
            {
                { "BMP", 0 },
                { "JPEG", 1 },
                { "PNG", 2 }
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
            if (App.IsAdmin())
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
            this.IsEnabledWaitGameEndMode = false;

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

        private async void LaunchGame(string gameId, int toolIndex, string toolName = "")
        {
            try
            {
                EnableLimitationMode(true);

                string gameProcessName;

                switch (toolIndex)
                {
                    case 0:
                        gameProcessName = await Task.Run(() 
                            => GameProcessHandler.StartGameProcess(gameId)
                            );
                        EnableWaitGameEndMode(gameProcessName);
                        break;
                    case 1:
                        gameProcessName = await Task.Run(() 
                            => GameProcessHandler.StartGameProcessWithTool(gameId, "vpatch.exe")
                            );
                        EnableWaitGameEndMode(gameProcessName);
                        break;
                    case 2:
                        string[] thpracFiles = GameFile.GetThpracFiles(gameId);
                        if (thpracFiles.Length == 1)
                        {
                            gameProcessName = await Task.Run(()
                                => GameProcessHandler.StartGameProcessWithTool(gameId, Path.GetFileName(thpracFiles[0]))
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
                                    => GameProcessHandler.StartGameProcessWithTool(gameId, thpracDialog.ThpracFileName)
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
                    case 3:
                        gameProcessName = await Task.Run(()
                            => GameProcessHandler.StartGameProcessWithTool(gameId, toolName)
                            );
                        EnableWaitGameEndMode(gameProcessName);
                        break;
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
            GameBasePrimaryComboBox.SelectedIndex = applicationSettings.SelectedGamePriorityIndex;
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
                GameComboBox.SelectedIndex = _gameDictionary[selectedGameId];
            }
            else
            {
                GameComboBox.SelectedIndex = 0;
            }

            string? themeName = applicationSettings.ThemeName;
            if (!string.IsNullOrEmpty(themeName))
            {
                ThemeSettingsComboBox.SelectedIndex = _themeDictionary[themeName];
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

            string captureFileDirectory = applicationSettings.CaptureFileDirectory;
            if (!string.IsNullOrEmpty (captureFileDirectory))
            {
                CaptureDirectoryPathBox.Text = captureFileDirectory;
            }
            else
            {
                CaptureDirectoryPathBox.Text = PathInfo.GameCaptureDirectory;
            }
            
            string captureFileFormat = applicationSettings.CaptureFileFormat;
            if (!string.IsNullOrEmpty (captureFileFormat))
            {
                CaptureFormatComboBox.SelectedIndex = _captureFileFormatDictionary[captureFileFormat];
            }
            else
            {
                CaptureFormatComboBox.SelectedIndex = 0;
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
                SelectedGamePriorityIndex = this.GameProcessPriorityIndex,
                AlwaysOnTop = AlwaysOnTopCheckBox.IsChecked == true,
                ResizeRateIndex = ResizeRateComboBox.SelectedIndex,
                ResizeByRate = ResizeByRateRadioButton.IsChecked == true,
                ResizeBySize = ResizeBySizeRadioButton.IsChecked == true,
                ResizeWidth = GameWindowWidthBox.Text,
                ResizeHeight = GameWindowHeightBox.Text,
                ThemeName = ApplicationTheme.ThemeName,
                CaptureFileDirectory = GameWindowHandler.CaptureFileDirectory,
                CaptureFileFormat = GameWindowHandler.CaptureFileFormat
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
            LaunchWithAnyToolMenuItem.IsEnabled = !enabled;
            SearchGameFileMenuItem.IsEnabled = !enabled;
            LaunchCustomProgramMenuItem.IsEnabled = !enabled;
            CatchGameProcessMenuItem.IsEnabled = !enabled;
            CommandGameLauncherMenuItem.IsEnabled = !enabled;

            GamePathBox.IsEnabled = !enabled;
            GamePathBrowseButton.IsEnabled = !enabled;
            SearchGameFileButton.IsEnabled = !enabled;
            LaunchGameButton.IsEnabled = !enabled;
            LaunchCustomProgramButton.IsEnabled = !enabled;
            CatchGameProcessButton.IsEnabled = !enabled;

            KillGameProcessMenuItem.IsEnabled = enabled;
            CaptureGameWindowMenuItem.IsEnabled = enabled;

            KillGameProcessButton.IsEnabled = enabled;
            CaptureGameWindowButton.IsEnabled = enabled;
            ResizeButton.IsEnabled = enabled;
        }

        private void EnableWaitGameEndMode(string gameProcessName)
        {
            this.GameProcessName= gameProcessName;
            this.GameStartTime= DateTime.Now;
            this.IsEnabledWaitGameEndMode = true;

            AppStatusBlock.Content = $"{GameIndex.GetGameName(this.GameId)}を実行中...";

            if (this.GameProcessPriorityIndex > 0)
            {
                try
                {
                    GameProcessHandler.SetGamePriority(this.GameProcessName, this.GameProcessPriorityIndex);
                }
                catch (Exception) 
                {

                }
            }

            _gameWaitingWorker = new BackgroundWorker();
            _gameWaitingWorker.DoWork += new DoWorkEventHandler(WorkerDowork);
            _gameWaitingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerRunningComplete);
            _gameWaitingWorker.RunWorkerAsync(gameProcessName);

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
                    int patchIndex = commandDialog.ToolIndex;
                    if (!string.IsNullOrEmpty(gameId))
                    {
                        if (_gameDictionary.ContainsKey(gameId))
                        {
                            int index = _gameDictionary[gameId];
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
            this.IsEnabledWaitGameEndMode = false;
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

        private void LaunchWithAnyToolMenuItemClick(object sender, RoutedEventArgs e)
        {
            SelectToolDialog selectToolDialog = new()
            {
                Owner = this,
                GameId = this.GameId
            };

            if (selectToolDialog.ShowDialog() == true)
            {
                string toolName = selectToolDialog.ToolName;
                LaunchGame(this.GameId, 3, toolName);
            }
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
                GameComboBox.SelectedIndex = _gameDictionary[gameId];
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
            if (this.IsEnabledWaitGameEndMode)
            {
                Forms.DialogResult result
                    = Forms.MessageBox.Show(
                    $"ゲーム終了待機モードです。\n本当に{_appName}を終了させますか。", _appName,
                    Forms.MessageBoxButtons.YesNo,
                    Forms.MessageBoxIcon.Exclamation,
                    Forms.MessageBoxDefaultButton.Button2);
                if (result == Forms.DialogResult.Yes)
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

            GameRunningTimeBlock.Text = "00min00sec";
            GameStartTimeBlock.Text = string.Empty;
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
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.P && !this.IsEnabledWaitGameEndMode)
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
                GameRunningTimeStaticsDialog gameRunningTimeStaticsDialog = new()
                {
                    GamePlayLogDatas = GameLogDataGrid.DataContext,
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

        private void BrowseCaptureDirectoryButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new();
            if (openFolderDialog.ShowDialog() == true)
            {
                CaptureDirectoryPathBox.Text = openFolderDialog.FolderName;
            }
        }

        private void CaptureDirectoryPathBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            GameWindowHandler.CaptureFileDirectory = CaptureDirectoryPathBox.Text;
        }

        private void CaptureFormatComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)CaptureFormatComboBox.SelectedItem;
            string captureFileFormat = item.Uid;

            GameWindowHandler.CaptureFileFormat = captureFileFormat;
        }

        private void CaptureGameWindowMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(GameWindowHandler.CaptureFileDirectory))
                {
                    GameWindowHandler.GetGameWindowCapture(this.GameProcessName);
                }
                else
                {
                    OpenFolderDialog openFolderDialog = new()
                    {
                        Title = "キャプチャファイルの保存フォルダを指定してください。"
                    };

                    if (openFolderDialog.ShowDialog() == true)
                    {
                        GameWindowHandler.CaptureFileDirectory = openFolderDialog.FolderName;
                    }
                    else
                    {
                        MessageBox.Show(this, "キャプチャがキャンセルされました。", "ゲームウィンドウのキャプチャ",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitApplicationMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenApplicationDirectoryMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _ = Process.Start("explorer.exe", PathInfo.AppLocation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AboutDinamicAero2MenuItemClick(object sender, RoutedEventArgs e)
        {
            string dynamicAero2DllPath = PathInfo.DynamicAero2DllPath;

            string dllName = FileVersionInfo.GetVersionInfo(dynamicAero2DllPath).ProductName;
            string dllVersion = FileVersionInfo.GetVersionInfo(dynamicAero2DllPath).FileVersion;
            string dllDeveloper = FileVersionInfo.GetVersionInfo(dynamicAero2DllPath).CompanyName;
            string dllCopyright = FileVersionInfo.GetVersionInfo(dynamicAero2DllPath).LegalCopyright;

            string dllInformation = $"{dllName}\nVersion.{dllVersion}\nby {dllDeveloper}\n{dllCopyright}";

            MessageBox.Show(this, dllInformation, "DynamicAero2 について",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SystemInformationMenuItemClick(object sender, RoutedEventArgs e)
        {
            SystemInformationDialog systemInformationDialog = new()
            {
                Owner = this
            };

            systemInformationDialog.ShowDialog();
        }

        private void OpenGameCaptureDirectoryMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", GameWindowHandler.CaptureFileDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GameBasePrimaryComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.GameProcessPriorityIndex = GameBasePrimaryComboBox.SelectedIndex;
            if (this.IsEnabledWaitGameEndMode)
            {
                try
                {
                    if (this.GameProcessPriorityIndex > 0)
                    {
                        GameProcessHandler.SetGamePriority(this.GameProcessName, this.GameProcessPriorityIndex);
                    }
                    else
                    {
                        GameProcessHandler.SetGamePriority(this.GameProcessName, 3);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"ゲームプロセスの基本優先度の変更に失敗\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MainTabControlTabItemMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int index = MainTabControl.SelectedIndex;

            //マウスホイール回転方向の判定
            if (e.Delta > 0)
            {
                if (index == 0)
                {
                    MainTabControl.SelectedIndex = 2;
                }
                else
                {
                    MainTabControl.SelectedIndex = index - 1;
                }
            }
            else
            {
                if (index == 2)
                {
                    MainTabControl.SelectedIndex = 0;
                }
                else
                {
                    MainTabControl.SelectedIndex = index + 1;
                }
            }
        }

        private void SendFeedbackMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo url = new()
                {
                    FileName = "https://forms.office.com/r/9vsUThbWN0",
                    UseShellExecute = true
                };

                _ = Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditVsyncPatchIniMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameFilePath = GameFile.GetGameFilePath(this.GameId);
            string gameDirectoryPath = Path.GetDirectoryName(gameFilePath);

            string vsyncPathIniFilePath = $"{gameDirectoryPath}\\vpatch.ini";

            EditVpatchIniDialog editVpatchIniDialog = new()
            {
                Owner = this,
                VsyncPatchIniFilePath = vsyncPathIniFilePath
            };

            editVpatchIniDialog.ShowDialog();
        }

        private void SearchGameFileMenuItemClick(object sender, RoutedEventArgs e)
        {
            SearchGameFilesDialog searchGameFilesDialog = new()
            {
                Owner = this
            };
            searchGameFilesDialog.ShowDialog();

            GamePathBox.Text = GameFile.GetGameFilePath(this.GameId);
        }

        private void OpenWebSiteMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo url = new()
                {
                    FileName = "https://armadillo-winx.github.io/ThLaunchSite/",
                    UseShellExecute = true
                };

                _ = Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
