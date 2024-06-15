global using System;
global using System.Diagnostics;
global using System.IO;
global using System.Windows;

global using ThLaunchSite.Exceptions;
global using ThLaunchSite.Game;
global using ThLaunchSite.Plugin;
global using ThLaunchSite.Settings;

using DxLibDLL;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
        private string? _gameId;
        private string? _gameName;
        private string? _applicationLanguage;

        private string GameId
        {
            get
            {
                return _gameId;
            }

            set
            {
                _gameId = value;

                this.GameName = GameIndex.GetGameName(value);
                SubTitleBlock.Text = GameIndex.GetGameSubTitle(value);

                ThNumberSymbolImage.Source = new BitmapImage();

                GameRunningTimeBlock.Text = "00min00sec";

                if (!string.IsNullOrEmpty(value))
                {
                    LaunchWithVpatchMenuItem.IsEnabled = GameIndex.IsVsyncPatchAvailable(value);
                    LaunchWithVpatchButton.IsEnabled = GameIndex.IsVsyncPatchAvailable(value);
                    EditVsyncPatchIniMenuItem.IsEnabled = GameIndex.IsVsyncPatchAvailable(value);
                    EditVsyncPatchIniButton.IsEnabled = GameIndex.IsVsyncPatchAvailable(value);

                    if (GameIndex.GetGameEdition(value) == "Trial")
                    {
                        TrialImage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        TrialImage.Visibility = Visibility.Hidden;
                    }

                    try
                    {
                        ThNumberSymbolImage.Source
                            = new BitmapImage(new Uri($"pack://application:,,,/ThLaunchSite;component/ThNumberSymbols/{value}.png"));
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    try
                    {
                        ThNumberSymbolImage.Source
                            = new BitmapImage(new Uri($"pack://application:,,,/ThLaunchSite;component/ThNumberSymbols/Th00.png"));
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private string GameName
        {
            get
            {
                return _gameName;
            }

            set
            {
                _gameName = value;
                if (!string.IsNullOrEmpty(value))
                {
                    GameTitleBlock.Text = value;
                }
                else
                {
                    GameTitleBlock.Text = Properties.Resources.MessageNoGameSelected;
                }
            }
        }

        private Process GameProcess { get; set; }
        private int GameProcessPriorityIndex { get; set; }

        private DateTime GameStartTime { get; set; }

        private bool IsWaitGameEndModeEnabled { get; set; }

        private string? ApplicationLanguage
        {
            get
            {
                return _applicationLanguage;
            }

            set
            {
                _applicationLanguage = value;

                if (value == "Auto" || string.IsNullOrEmpty(value))
                {
                    CultureInfo culture = CultureInfo.CurrentCulture;
                    ResourceService.Current.ChangeCulture(culture.Name);
                }
                else
                {
                    ResourceService.Current.ChangeCulture(value);
                }

                if (App.IsAdmin())
                {
                    AuthorityBlock.Content = Properties.Resources.StatusAdministrator;
                }
                else
                {
                    AuthorityBlock.Content = Properties.Resources.StatusUser;
                }

                SetGameSelectionMenu();

                GetExternalTools();

                if (!this.IsWaitGameEndModeEnabled)
                {
                    AppStatusBlock.Content = Properties.Resources.StatusReady;
                }
                else
                {
                    AppStatusBlock.Content = Properties.Resources.StatusGameRunnning;
                }

                if (string.IsNullOrEmpty(this.GameName))
                {
                    GameTitleBlock.Text = Properties.Resources.MessageNoGameSelected;
                }
            }
        }

        private AboutDialog? _aboutDialog = null;
        private BackgroundWorker? _gameWaitingWorker = null;
        private DispatcherTimer? _gameControlTimer = null;

        private readonly string? _appName = VersionInfo.AppName;
        private readonly string? _appVersion = VersionInfo.AppVersion;
        private readonly string? _settingsDirectory = PathInfo.SettingsDirectory;
        private readonly string _gamePlayLogFile = PathInfo.GamePlayLogFile;

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

        private readonly Dictionary<string, int> _languageDictionary
            = new()
            {
                { "Auto", 0 },
                { "en-US", 1 },
                { "ja-JP", 2 }
            };

        public MainWindow()
        {
            InitializeComponent();

            string appProcessName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcessesByName(appProcessName).Length > 1)
            {
                MessageBox.Show(this, Properties.Resources.ErrorMessageMultiActivation, Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            this.Title = _appName;

            this.GameId = string.Empty;
            this.GameName = string.Empty;
            this.IsWaitGameEndModeEnabled = false;

            this.GameProcess = new();

            TrialImage.Visibility = Visibility.Hidden;

            ShiftKeyToggleButton.IsChecked = false;
            ShiftKeyToggleButton.IsEnabled = false;
            ZKeyToggleButton.IsChecked = false;
            ZKeyToggleButton.IsEnabled = false;
            XKeyToggleButton.IsChecked = false;
            XKeyToggleButton.IsEnabled = false;
            UpKeyToggleButton.IsChecked = false;
            UpKeyToggleButton.IsEnabled = false;
            DownKeyToggleButton.IsChecked = false;
            DownKeyToggleButton.IsEnabled = false;
            LeftKeyToggleButton.IsChecked = false;
            LeftKeyToggleButton.IsEnabled = false;
            RightKeyToggleButton.IsChecked = false;
            RightKeyToggleButton.IsEnabled = false;

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
                        $"{Properties.Resources.ErrorMessageFailedToCreateSettingsDirectory}\n\n{ex.Message}",
                        Properties.Resources.TitleError,
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
                        $"{Properties.Resources.ErrorMessageFailedToCreateExternalToolsConfigFile}\n\n{ex.Message}",
                        Properties.Resources.TitleError,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (!Directory.Exists(PathInfo.PluginDirectory))
            {
                try
                {
                    Directory.CreateDirectory(PathInfo.PluginDirectory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"{Properties.Resources.ErrorMessageFailedToCreatePluginDirecoty}\n\n{ex.Message}",
                        Properties.Resources.TitleError,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                ConfigurePlugins();
            }

            try
            {
                SettingsConfiguration.ConfigureGameEditionSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Properties.Resources.ErrorMessageFailedToCreateGameEditionConfigFile}\n{ex.Message}", Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (File.Exists(PathInfo.GamePathSettingsFile))
            {
                try
                {
                    SettingsConfiguration.ConfigureGamePathSettings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Properties.Resources.ErrorMessageFailedToCreateGamePathConfigFile}\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.MessageToSetGamePath, _appName,
                    MessageBoxButton.OK, MessageBoxImage.Information);
                GamePathSettingsDialog gamePathSettingsDialog = new();
                gamePathSettingsDialog.ShowDialog();

                List<string> enabledGamesList = GameIndex.GetAvailableGamesList();

                if (enabledGamesList.Count > 0)
                {
                    this.GameId = enabledGamesList[0];
                }
            }

            try
            {
                SetBackupGameListBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Properties.Resources.ErrorMessageFailedToReadBackupDirectory}\n{ex.Message}", Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ViewGamePlayLogData();

            try
            {
                ConfigureApplicationSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Properties.Resources.ErrorMessageFailedToConfigApplicationSettings}\n{ex.Message}", Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);

                ThemeSettingsComboBox.SelectedIndex = 0;
                LanguageComboBox.SelectedIndex = 0;
            }
        }

        private async void LaunchGame(string gameId, int toolIndex, string toolName = "")
        {
            try
            {
                EnableLimitationMode(true);

                Process gameProcess;

                switch (toolIndex)
                {
                    case 0:
                        gameProcess = await Task.Run(()
                            => GameProcessHandler.StartGameProcess(gameId)
                            );
                        EnableWaitGameEndMode(gameProcess);
                        break;
                    case 1:
                        gameProcess = await Task.Run(()
                            => GameProcessHandler.StartGameProcessWithApplyingTool(gameId, "vpatch.exe")
                            );
                        EnableWaitGameEndMode(gameProcess);
                        break;
                    case 2:
                        LaunchGameWithApplyingThprac(gameId);
                        break;
                    case 3:
                        gameProcess = await Task.Run(()
                            => GameProcessHandler.StartGameProcessWithApplyingTool(gameId, toolName)
                            );
                        EnableWaitGameEndMode(gameProcess);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"{Properties.Resources.ErrorMessageFailedToStartGameProcess}\n{ex.Message}",
                    Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
                EnableLimitationMode(false);
            }
        }

        private async void LaunchGameWithApplyingThprac(string gameId)
        {
            Process gameProcess;
            string[] thpracFiles = GameFile.GetThpracFiles(gameId);
            if (thpracFiles.Length == 1)
            {
                gameProcess = await Task.Run(()
                    => GameProcessHandler.StartGameProcessWithApplyingTool(gameId, Path.GetFileName(thpracFiles[0]))
                );
                EnableWaitGameEndMode(gameProcess);
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
                    gameProcess = await Task.Run(()
                        => GameProcessHandler.StartGameProcessWithApplyingTool(gameId, thpracDialog.ThpracFileName)
                    );
                    EnableWaitGameEndMode(gameProcess);
                }
                else
                {
                    EnableLimitationMode(false);
                }
            }
            else
            {
                _ = MessageBox.Show(this, Properties.Resources.ErrorMessageFailedToFindThpracFiles, Properties.Resources.TitleError,
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
                    ObservableCollection<GamePlayLogData> gamePlayLogDatas = [];
                    gamePlayLogDatas = GamePlayLogRecorder.GetGamePlayLogDatas();
                    GameLogDataGrid.AutoGenerateColumns = false;
                    GameLogDataGrid.DataContext = gamePlayLogDatas;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"{Properties.Resources.ErrorMessageFailedToGetGameExecutionLog}\n{ex.Message}",
                        Properties.Resources.TitleError,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ConfigurePlugins()
        {
            try
            {
                PluginHandler.GetPlugin();

                if (PluginHandler.GameFilesPlugins != null && PluginHandler.GameFilesPlugins.Count > 0)
                    SetGameFilesPluginMenu(PluginHandler.GameFilesPlugins);

                if (PluginHandler.GameScoreFilesPlugins != null && PluginHandler.GameScoreFilesPlugins.Count > 0)
                    SetGameScoreFilesPluginMenu(PluginHandler.GameScoreFilesPlugins);

                if (PluginHandler.GamePlayLogPlugins != null && PluginHandler.GamePlayLogPlugins.Count > 0)
                    SetGamePlayLogPluginMenu(PluginHandler.GamePlayLogPlugins);

                if (PluginHandler.SelectedGamePlugins != null && PluginHandler.SelectedGamePlugins.Count > 0)
                    SetSelectedGamePluginMenu(PluginHandler.SelectedGamePlugins);

                if (PluginHandler.ToolPlugins != null && PluginHandler.ToolPlugins.Count > 0)
                    SetToolPluginMenu(PluginHandler.ToolPlugins);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{Properties.Resources.ErrorMessageFailedToGetPlugins}\n\n{ex.Message}",
                    Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetGameFilesPluginMenu(List<dynamic> gameFilesPlugins)
        {
            foreach (dynamic gameFilesPlugin in gameFilesPlugins)
            {
                try
                {
                    gameFilesPlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = gameFilesPlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    List<string> availableGamesList = GameIndex.GetAvailableGamesList();
                    Dictionary<string, string> availableGameFilesDictionary = [];
                    if (availableGamesList.Count > 0)
                    {
                        foreach (string gameId in availableGamesList)
                        {
                            availableGameFilesDictionary.Add(gameId, GameFile.GetGameFilePath(gameId));
                        }
                    }

                    gameFilesPlugin.Main(availableGamesList, availableGameFilesDictionary);
                };

                ToolsMenuItem.Items.Add(menuItem);
            }
        }

        private void SetGameScoreFilesPluginMenu(List<dynamic> gameScoreFilesPlugins)
        {
            foreach (dynamic gameScoreFilesPlugin in gameScoreFilesPlugins)
            {
                try
                {
                    gameScoreFilesPlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = gameScoreFilesPlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    List<string> availableGamesList = GameIndex.GetAvailableGamesList();
                    Dictionary<string, string> availableGameScoreFilesDictionary = [];
                    if (availableGamesList.Count > 0)
                    {
                        foreach (string gameId in availableGamesList)
                        {
                            availableGameScoreFilesDictionary.Add(gameId, GameFile.GetScoreFilePath(gameId));
                        }
                    }

                    gameScoreFilesPlugin.Main(availableGamesList, availableGameScoreFilesDictionary);
                };

                ToolsMenuItem.Items.Add(menuItem);
            }
        }

        private void SetSelectedGamePluginMenu(List<dynamic> selectedGamePlugins)
        {
            foreach (dynamic selectedGamePlugin in selectedGamePlugins)
            {
                try
                {
                    selectedGamePlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = selectedGamePlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    selectedGamePlugin.Main(this.GameId, GameFile.GetGameFilePath(this.GameId));
                };

                ToolsMenuItem.Items.Add(menuItem);
            }
        }

        private void SetGamePlayLogPluginMenu(List<dynamic> gamePlayLogPlugins)
        {
            foreach (dynamic gamePlayLogPlugin in gamePlayLogPlugins)
            {
                try
                {
                    gamePlayLogPlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = gamePlayLogPlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    gamePlayLogPlugin.Main(PathInfo.GamePlayLogFile);
                };

                ToolsMenuItem.Items.Add(menuItem);
            }
        }

        private void SetToolPluginMenu(List<dynamic> toolPlugins)
        {
            foreach (dynamic toolPlugin in toolPlugins)
            {
                try
                {
                    toolPlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = toolPlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    toolPlugin.Main();
                };

                ToolsMenuItem.Items.Add(menuItem);
            }
        }

        private void SetGameSelectionMenu()
        {
            SelectGameButtonContextMenu.Items.Clear();

            List<string> enabledGamesList = GameIndex.GetAvailableGamesList();
            
            if (enabledGamesList.Count > 0)
            {
                foreach (string gameId in enabledGamesList)
                {
                    string gameName = GameIndex.GetGameName(gameId);

                    MenuItem gameMenuItem = new()
                    {
                        Header = $"{gameId}: {gameName}",
                        Uid = gameId
                    };

                    if (GameIndex.GetGameEdition(gameId) == "Trial")
                    {
                        try
                        {
                            gameMenuItem.Icon =
                            new Image
                            {
                                Source = new BitmapImage(new Uri("pack://application:,,,/ThLaunchSite;component/Images/TR.png"))
                            };
                        }
                        catch (Exception)
                        {

                        }
                    }

                    gameMenuItem.Click += new RoutedEventHandler(GameSelectionMenuItemClick);

                    SelectGameButtonContextMenu.Items.Add(gameMenuItem);
                }
            }

            Separator separator = new();
            SelectGameButtonContextMenu.Items.Add(separator);

            MenuItem gamePathSettingsMenuItem = new()
            {
                Header = Properties.Resources.MenuSetGamePath,
                Icon = 
                new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/ThLaunchSite;component/Images/Settings.png"))
                }
            };
            gamePathSettingsMenuItem.Click += new RoutedEventHandler(SetGamePathMenuItemClick);
            SelectGameButtonContextMenu.Items.Add(gamePathSettingsMenuItem);
        }

        private void GameSelectionMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = ((MenuItem)sender).Uid;
                if (!string.IsNullOrEmpty(gameId))
                {
                    this.GameId = gameId;
                }
                else
                {
                    MessageBox.Show(this, Properties.Resources.ErrorMessageInvalidSelection, Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this, Properties.Resources.ErrorMessageInvalidSelection,
                    Properties.Resources.ErrorMessageInvalidSelection,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetBackupGameListBox()
        {
            BackupGameListBox.Items.Clear();
            BackupListBox.Items.Clear();

            if (Directory.Exists(PathInfo.ScoreBackupDirectory))
            {
                string[] backupDirectories
                    = Directory.GetDirectories(PathInfo.ScoreBackupDirectory, "*", SearchOption.TopDirectoryOnly);

                foreach (string backupDirectory in backupDirectories)
                {
                    string directoryName = Path.GetFileName(backupDirectory);
                    BackupGameListBox.Items.Add(GameIndex.GetGameName(directoryName));
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
            AutoBackupCheckBox.IsChecked = applicationSettings.AutoBackup;
            
            GameWindowWidthBox.Text = applicationSettings.ResizeWidth;
            GameWindowHeightBox.Text = applicationSettings.ResizeHeight;

            if (!string.IsNullOrEmpty(applicationSettings.SelectedGameId))
            {
                this.GameId = applicationSettings.SelectedGameId;
            }

            string? themeName = applicationSettings.ThemeName;
            if (!string.IsNullOrEmpty(themeName))
            {
                try
                {
                    ThemeSettingsComboBox.SelectedIndex = _themeDictionary[themeName];
                }
                catch (Exception)
                {
                    ThemeSettingsComboBox.SelectedIndex = 0;
                }
            }
            else
            {
                ThemeSettingsComboBox.SelectedIndex = 0;
            }

            string? languageId = applicationSettings.LanguageId;
            if (!string.IsNullOrEmpty(languageId))
            {
                try
                {
                    LanguageComboBox.SelectedIndex = _languageDictionary[languageId];
                }
                catch (Exception)
                {
                    LanguageComboBox.SelectedIndex = 0;
                }
            }
            else
            {
                LanguageComboBox.SelectedIndex = 0;
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
            if (!string.IsNullOrEmpty(captureFileDirectory))
            {
                CaptureDirectoryPathBox.Text = captureFileDirectory;
            }
            else
            {
                CaptureDirectoryPathBox.Text = PathInfo.GameCaptureDirectory;
            }

            string captureFileFormat = applicationSettings.CaptureFileFormat;
            if (!string.IsNullOrEmpty(captureFileFormat))
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
                AutoBackup = AutoBackupCheckBox.IsChecked == true,
                ResizeWidth = GameWindowWidthBox.Text,
                ResizeHeight = GameWindowHeightBox.Text,
                ThemeName = ApplicationTheme.ThemeName,
                LanguageId = this.ApplicationLanguage,
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
                            Header = Properties.Resources.MenuNone,
                            IsEnabled = false
                        };
                        ExternalToolsMenuItem.Items.Add(item);
                    }
                }
                catch (Exception)
                {
                    MenuItem item = new()
                    {
                        Header = Properties.Resources.MenuNone,
                        IsEnabled = false
                    };
                    ExternalToolsMenuItem.Items.Add(item);
                }
            }
            else
            {
                MenuItem item = new()
                {
                    Header = Properties.Resources.MenuNone,
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
                ExternalTool.StartExternalToolProcess(name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnableLimitationMode(bool enabled)
        {
            SelectGameButton.IsEnabled = !enabled;
            LaunchGameMenuItem.IsEnabled = !enabled;
            if (GameIndex.IsVsyncPatchAvailable(this.GameId))
            {
                LaunchWithVpatchMenuItem.IsEnabled = !enabled;
                LaunchWithVpatchButton.IsEnabled = !enabled;
                EditVsyncPatchIniMenuItem.IsEnabled = !enabled;
                EditVsyncPatchIniButton.IsEnabled = !enabled;
            }
            else
            {
                LaunchWithVpatchMenuItem.IsEnabled = false;
                LaunchWithVpatchButton.IsEnabled= false;
                EditVsyncPatchIniMenuItem.IsEnabled = false;
                EditVsyncPatchIniButton.IsEnabled = false;
            }
            LaunchWithThpracMenuItem.IsEnabled = !enabled;
            LaunchWithThpracButton.IsEnabled = !enabled;
            LaunchWithAnyToolMenuItem.IsEnabled = !enabled;
            SearchGameFileMenuItem.IsEnabled = !enabled;
            SetGamePathMenuItem.IsEnabled = !enabled;
            LaunchCustomProgramMenuItem.IsEnabled = !enabled;
            InstallToolsMenuItem.IsEnabled = !enabled;

            LaunchGameButton.IsEnabled = !enabled;
            LaunchCustomProgramButton.IsEnabled = !enabled;
            SetGamePathButton.IsEnabled = !enabled;

            BackupGameListBox.IsEnabled = !enabled;
            BackupListBox.IsEnabled = !enabled;
            RestoreFromBackupButton.IsEnabled = !enabled;
            DeleteBackupButton.IsEnabled = !enabled;

            KillGameProcessMenuItem.IsEnabled = enabled;
            CaptureGameWindowMenuItem.IsEnabled = enabled;

            KillGameProcessButton.IsEnabled = enabled;
            CaptureGameWindowButton.IsEnabled = enabled;
            ResizeButton.IsEnabled = enabled;
            GameAudioControlSlider.IsEnabled = enabled;
        }

        private void EnableWaitGameEndMode(Process gameProcess)
        {
            EnableLimitationMode(true);

            this.GameProcess = gameProcess;
            this.GameStartTime = DateTime.Now;
            this.IsWaitGameEndModeEnabled = true;

            AppStatusBlock.Content = Properties.Resources.StatusGameRunnning;

            if (this.GameProcessPriorityIndex > 0)
            {
                try
                {
                    GameProcessHandler.SetGamePriority(this.GameProcess, this.GameProcessPriorityIndex);
                }
                catch (Exception)
                {

                }
            }

            if (PluginHandler.GameProcessPlugins != null && PluginHandler.GameProcessPlugins.Count > 0)
            {
                foreach (dynamic gameProcessPlugin in PluginHandler.GameProcessPlugins)
                {
                    try
                    {
                        gameProcessPlugin.MainWindow = this;
                    }
                    catch (Exception) { }

                    gameProcessPlugin.Main(this.GameId, this.GameProcess.ProcessName);
                }
            }

            _gameWaitingWorker = new BackgroundWorker();
            _gameWaitingWorker.DoWork += new DoWorkEventHandler(WorkerDoWork);
            _gameWaitingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerRunningComplete);
            _gameWaitingWorker.RunWorkerAsync(gameProcess);

            _gameControlTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };

            _gameControlTimer.Tick += (e, s) =>
            {
                TimeSpan time = DateTime.Now - this.GameStartTime;

                string gameRunningTime = time.ToString(@"mm\m\i\nss\s\e\c");
                GameRunningTimeBlock.Text = gameRunningTime;

                float gameAudioVolume = 0;
                try
                {
                    gameAudioVolume = GameAudio.GetGameProcessAudioVolume(this.GameProcess);
                }
                catch (Exception)
                {
                    gameAudioVolume = 0;
                }

                GameAudioControlSlider.Value = gameAudioVolume * 100;

                ShiftKeyToggleButton.IsChecked = DX.CheckHitKey(DX.KEY_INPUT_LSHIFT) == 1 || DX.CheckHitKey(DX.KEY_INPUT_RSHIFT) == 1;
                ShiftKeyToggleButton.IsEnabled = DX.CheckHitKey(DX.KEY_INPUT_LSHIFT) == 1 || DX.CheckHitKey(DX.KEY_INPUT_RSHIFT) == 1;
                ZKeyToggleButton.IsChecked = DX.CheckHitKey(DX.KEY_INPUT_Z) == 1;
                ZKeyToggleButton.IsEnabled = DX.CheckHitKey(DX.KEY_INPUT_Z) == 1;
                XKeyToggleButton.IsChecked = DX.CheckHitKey(DX.KEY_INPUT_X) == 1;
                XKeyToggleButton.IsEnabled = DX.CheckHitKey(DX.KEY_INPUT_X) == 1;
                UpKeyToggleButton.IsChecked = DX.CheckHitKey(DX.KEY_INPUT_UP) == 1 || DX.CheckHitKey(DX.KEY_INPUT_NUMPAD8) == 1;
                UpKeyToggleButton.IsEnabled = DX.CheckHitKey(DX.KEY_INPUT_UP) == 1 || DX.CheckHitKey(DX.KEY_INPUT_NUMPAD8) == 1;
                DownKeyToggleButton.IsChecked = DX.CheckHitKey(DX.KEY_INPUT_DOWN) == 1 || DX.CheckHitKey(DX.KEY_INPUT_NUMPAD2) == 1;
                DownKeyToggleButton.IsEnabled = DX.CheckHitKey(DX.KEY_INPUT_DOWN) == 1 || DX.CheckHitKey(DX.KEY_INPUT_NUMPAD2) == 1;
                LeftKeyToggleButton.IsChecked = DX.CheckHitKey(DX.KEY_INPUT_LEFT) == 1 || DX.CheckHitKey(DX.KEY_INPUT_NUMPAD4) == 1;
                LeftKeyToggleButton.IsEnabled = DX.CheckHitKey(DX.KEY_INPUT_LEFT) == 1 || DX.CheckHitKey(DX.KEY_INPUT_NUMPAD4) == 1;
                RightKeyToggleButton.IsChecked = DX.CheckHitKey(DX.KEY_INPUT_RIGHT) == 1 || DX.CheckHitKey(DX.KEY_INPUT_NUMPAD6) == 1;
                RightKeyToggleButton.IsEnabled = DX.CheckHitKey(DX.KEY_INPUT_RIGHT) == 1 || DX.CheckHitKey(DX.KEY_INPUT_NUMPAD6) == 1;
            };
            _gameControlTimer.Start();
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            Process gameProcess = (Process)e.Argument;
            gameProcess.WaitForExit();
        }

        private void WorkerRunningComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            this.IsWaitGameEndModeEnabled = false;
            _gameControlTimer.Stop();

            GameAudioControlSlider.Value = 0;

            ShiftKeyToggleButton.IsChecked = false;
            ShiftKeyToggleButton.IsEnabled = false;
            ZKeyToggleButton.IsChecked = false;
            ZKeyToggleButton.IsEnabled = false;
            XKeyToggleButton.IsChecked = false;
            XKeyToggleButton.IsEnabled = false;
            UpKeyToggleButton.IsChecked = false;
            UpKeyToggleButton.IsEnabled = false;
            DownKeyToggleButton.IsChecked = false;
            DownKeyToggleButton.IsEnabled = false;
            LeftKeyToggleButton.IsChecked = false;
            LeftKeyToggleButton.IsEnabled = false;
            RightKeyToggleButton.IsChecked = false;
            RightKeyToggleButton.IsEnabled = false;

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
                MessageBox.Show(this, $"{Properties.Resources.ErrorMessageFailedToGameExecutionLog}\n{ex.Message}",
                                Properties.Resources.TitleError,
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (AutoBackupCheckBox.IsChecked == true)
            {
                try
                {
                    GameScoreBackup.CreateScoreBackup(this.GameId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"{Properties.Resources.ErrorMessageFailedToCreateScoreBackup}\n{ex.Message}",
                                Properties.Resources.TitleError,
                                MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            try
            {
                SetBackupGameListBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Properties.Resources.ErrorMessageFailedToReadBackupDirectory}\n{ex.Message}",
                    Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            AppStatusBlock.Content = Properties.Resources.StatusReady;

            this.GameProcess.Dispose();

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

        private void LaunchCustomProgramMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = this.GameId;
                GameProcessHandler.StartCustomProgramProcess(gameId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"{Properties.Resources.ErrorMessageFailedToStartCustomProgram}\n{ex.Message}", Properties.Resources.TitleError,
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
                MessageBox.Show(this, ex.Message, Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void KillGameProcessMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                GameProcessHandler.KillGameProcess(this.GameProcess);
                MessageBox.Show(this, Properties.Resources.MessageToKillGameProcess, Properties.Resources.TitleKillGameProcess,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"{Properties.Resources.ErrorMessageFailedToKillGameProcess}\n{ex.Message}", Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (this.IsWaitGameEndModeEnabled)
            {
                Forms.DialogResult result
                    = Forms.MessageBox.Show(
                    Properties.Resources.MessageAskGameExit, _appName,
                    Forms.MessageBoxButtons.YesNo,
                    Forms.MessageBoxIcon.Exclamation,
                    Forms.MessageBoxDefaultButton.Button2);
                if (result == Forms.DialogResult.Yes)
                {
                    try
                    {
                        SettingsConfiguration.SaveGamePathSettings();
                        SettingsConfiguration.SaveGameEditionSettings();
                        SaveApplicationSettings();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{Properties.Resources.ErrorMessageFailedToSaveSettings}\n{ex.Message}", Properties.Resources.TitleError,
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
                    SettingsConfiguration.SaveGameEditionSettings();
                    SaveApplicationSettings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Properties.Resources.ErrorMessageFailedToSaveSettings}\n{ex.Message}", Properties.Resources.TitleError,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ResizeButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int resizeWidth = int.Parse(GameWindowWidthBox.Text);
                int resizeHeight = int.Parse(GameWindowHeightBox.Text);

                GameWindowHandler.ResizeWindow(this.GameProcess, resizeWidth, resizeHeight);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void ManageExternalToolsMenuItemClick(object sender, RoutedEventArgs e)
        {
            ManageExternalToolsDialog manageExternalToolsDialog = new()
            {
                Owner = this
            };

            manageExternalToolsDialog.ShowDialog();
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
                    GameWindowHandler.GetGameWindowCapture(this.GameId, this.GameProcess);
                }
                else
                {
                    OpenFolderDialog openFolderDialog = new()
                    {
                        Title = Properties.Resources.TitleToSetGameCaptureFolder
                    };

                    if (openFolderDialog.ShowDialog() == true)
                    {
                        GameWindowHandler.CaptureFileDirectory = openFolderDialog.FolderName;
                        GameWindowHandler.GetGameWindowCapture(this.GameId, this.GameProcess);
                    }
                    else
                    {
                        MessageBox.Show(this, Properties.Resources.MessageCaptureCanceled, Properties.Resources.TitleCaptureGameWindow,
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitApplicationMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AboutDynamicAero2MenuItemClick(object sender, RoutedEventArgs e)
        {
            string dynamicAero2DllPath = PathInfo.DynamicAero2DllPath;

            string dllName = FileVersionInfo.GetVersionInfo(dynamicAero2DllPath).ProductName;
            string dllVersion = FileVersionInfo.GetVersionInfo(dynamicAero2DllPath).FileVersion;
            string dllDeveloper = FileVersionInfo.GetVersionInfo(dynamicAero2DllPath).CompanyName;
            string dllCopyright = FileVersionInfo.GetVersionInfo(dynamicAero2DllPath).LegalCopyright;

            string dllInformation = $"{dllName}\nVersion.{dllVersion}\nby {dllDeveloper}\n{dllCopyright}";

            MessageBox.Show(this, dllInformation, Properties.Resources.TitleAboutDynamicAero2,
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
                if (Directory.Exists(GameWindowHandler.CaptureFileDirectory))
                {
                    Process.Start("explorer.exe", GameWindowHandler.CaptureFileDirectory);
                }
                else
                {
                    MessageBox.Show(this, Properties.Resources.ErrorMessageCaptureFolderNotFound, Properties.Resources.TitleError,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GameBasePrimaryComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.GameProcessPriorityIndex = GameBasePrimaryComboBox.SelectedIndex;
            if (this.IsWaitGameEndModeEnabled)
            {
                try
                {
                    if (this.GameProcessPriorityIndex > 0)
                    {
                        GameProcessHandler.SetGamePriority(this.GameProcess, this.GameProcessPriorityIndex);
                    }
                    else
                    {
                        GameProcessHandler.SetGamePriority(this.GameProcess, 3);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"{Properties.Resources.ErrorMessageFailedToChangeGameBasePrimary}\n{ex.Message}", Properties.Resources.TitleError,
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
                    MainTabControl.SelectedIndex = 4;
                }
                else
                {
                    MainTabControl.SelectedIndex = index - 1;
                }
            }
            else
            {
                if (index == 4)
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
                MessageBox.Show(this, ex.Message, Properties.Resources.TitleError,
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
        }

        private void InstallToolMenuItemClick(object sender, RoutedEventArgs e)
        {
            InstallToolsDialog installToolsDialog = new()
            {
                Owner = this
            };

            installToolsDialog.ShowDialog();
        }

        private void SelectGameButtonClick(object sender, RoutedEventArgs e)
        {
            if (!SelectGameButtonContextMenu.IsOpen)
            {
                SelectGameButtonContextMenu.PlacementTarget = SelectGameButton;
                SelectGameButtonContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                SelectGameButtonContextMenu.IsOpen = true;
            }
        }

        private void SetGamePathMenuItemClick(object sender, RoutedEventArgs e)
        {
            GamePathSettingsDialog gamePathSettingsDialog = new()
            {
                Owner = this
            };
            gamePathSettingsDialog.ShowDialog();

            SetGameSelectionMenu();
        }

        private void OpenGameLogFileMenuItemClick(object sender, RoutedEventArgs e)
        {
            string scoreFile = GameFile.GetScoreFilePath(this.GameId);
            string logFile = $"{Path.GetDirectoryName(scoreFile)}\\log.txt";

            TextViewer textViewer = new()
            {
                Owner = this,
                Title = "東方動作記録",
                FilePath = logFile,
                Encode = "shift_jis"
            };

            textViewer.ShowDialog();
        }

        private void BackupGameListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BackupListBox.Items.Clear();

            if (BackupGameListBox.Items.Count > 0 && BackupGameListBox.SelectedIndex > -1)
            {
                string selectedGameName = BackupGameListBox.SelectedItem as string;
                string gameId = GameIndex.GetGameIdFromGameName(selectedGameName);

                try
                {
                    string[] backupFiles = GameScoreBackup.GetScoreBackupFiles(gameId);
                    if (backupFiles.Length > 0)
                    {
                        int i = backupFiles.Length - 1;
                        while (true)
                        {
                            string backupFile = backupFiles[i];
                            string backupFileName = Path.GetFileName(backupFile);
                            BackupListBox.Items.Add(backupFileName);

                            if (i == 0)
                                break;

                            i--;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, Properties.Resources.TitleError,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RestoreFromBackupButtonClick(object sender, RoutedEventArgs e)
        {
            if (BackupListBox.SelectedIndex > -1)
            {
                try
                {
                    string selectedGameName = BackupGameListBox.SelectedItem as string;
                    string gameId = GameIndex.GetGameIdFromGameName(selectedGameName);
                    string backupFile = BackupListBox.SelectedItem as string;

                    string backupFileDate = backupFile.Replace(".bak", "").Split('_')[0].Replace("-", "/");
                    string backupFileTime = backupFile.Replace(".bak", "").Split('_')[1].Replace("-", ":");

                    MessageBoxResult result = MessageBox.Show(
                        this,
                        $"{GameIndex.GetGameName(gameId)}{Properties.Resources.MessageRestoreFromBackup}\n{Properties.Resources.MessageBackupFileName} {backupFile}\n{Properties.Resources.MessageBackupCreationDate} {backupFileDate} {backupFileTime}\n{Properties.Resources.MessageAskIsItOk}",
                        Properties.Resources.TitleRestoreFromBackup,
                        MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        GameScoreBackup.RestoreFromScoreBackup(gameId, backupFile);

                        MessageBox.Show(this, Properties.Resources.MessageRestore, Properties.Resources.TitleRestoreFromBackup,
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"{Properties.Resources.ErrorMessageFailedToRestoreFromBackup}\n{ex.Message}", Properties.Resources.TitleError,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(this, Properties.Resources.MessageSelectBackupFileToRestoreFrom, Properties.Resources.TitleRestoreFromBackup,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DeleteBackupButtonClick(object sender, RoutedEventArgs e)
        {
            if (BackupListBox.SelectedIndex > -1)
            {
                try
                {
                    string selectedGameName = BackupGameListBox.SelectedItem as string;
                    string gameId = GameIndex.GetGameIdFromGameName(selectedGameName);
                    string backupFile = BackupListBox.SelectedItem as string;

                    MessageBoxResult result = MessageBox.Show(
                        this, $"'{backupFile}' {Properties.Resources.MessageAskDelete}", Properties.Resources.TitleDeleteBackup,
                        MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        GameScoreBackup.DeleteScoreBackupFile(gameId, backupFile);
                        BackupListBox.Items.Remove(backupFile);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"{Properties.Resources.ErrorMessageFailedToDeleteBackup}\n{ex.Message}", Properties.Resources.TitleError,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(this, Properties.Resources.MessageSelectBackupFileToDelete, Properties.Resources.TitleDeleteBackup,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void WindowResizerPresetButtonClick(object sender, RoutedEventArgs e)
        {
            if (!WindowResizerPresetMenu.IsOpen) 
            {
                WindowResizerPresetMenu.PlacementTarget = WindowResizerPresetButton;
                WindowResizerPresetMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                WindowResizerPresetMenu.IsOpen = true;
            }
        }

        private void WindowResizerPresetMenuItemClick(object sender, RoutedEventArgs e)
        {
            //MenuItemのHeaderプロパティを取得
            string size = ((MenuItem)sender).Header.ToString();
            string[] sizes = size.Split('x');
            GameWindowWidthBox.Text = sizes[0];
            GameWindowHeightBox.Text = sizes[1];
        }

        private void LanguageComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)LanguageComboBox.SelectedItem;
            string languageId = (string)item.Uid;

            this.ApplicationLanguage = languageId;
        }

        private void CreateSelectedGameScoreBackupMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                bool result = GameScoreBackup.CreateScoreBackup(this.GameId);
                if (result)
                {
                    MessageBox.Show(this, Properties.Resources.MessageCreated, Properties.Resources.TitleCreateScoreBackup,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(this, Properties.Resources.MessageScoreFileNotFound,
                        Properties.Resources.TitleCreateScoreBackup,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message,
                    Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateAllGamesScoreBackupMenuItemClick(object sender, RoutedEventArgs e)
        {
            CreateAllGamesScoreBackupDialog createAllGamesScoreBackupDialog = new()
            {
                Owner = this
            };

            createAllGamesScoreBackupDialog.ShowDialog();
            try
            {
                SetBackupGameListBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, 
                    $"{Properties.Resources.ErrorMessageFailedToReadBackupDirectory}\n{ex.Message}",
                    Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PluginManagerMenuItemClick(object sender, RoutedEventArgs e)
        {
            ManagePluginDialog managePluginDialog = new()
            {
                Owner = this
            };

            managePluginDialog.ShowDialog();
        }

        private void AboutPluginBaseMenuItemClick(object sender, RoutedEventArgs e)
        {
            string pluginBaseLibrary = $"{PathInfo.AppLocation}\\ThLaunchSite.Plugin.dll";
            if (File.Exists(pluginBaseLibrary))
            {
                string productName = FileVersionInfo.GetVersionInfo(pluginBaseLibrary).ProductName;
                string productVersion = FileVersionInfo.GetVersionInfo(pluginBaseLibrary).ProductVersion;
                string developer = FileVersionInfo.GetVersionInfo(pluginBaseLibrary).CompanyName;
                MessageBox.Show(this,
                    $"{productName}\nVersion.{productVersion}\nby {developer}",
                    Properties.Resources.TitlePlugInBaseVersion,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GameAudioControlSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsWaitGameEndModeEnabled)
            {
                GameAudioVolumeBlock.Text = ((int)(GameAudioControlSlider.Value)).ToString();

                try
                {
                    float gameAudioVolume = GameAudio.GetGameProcessAudioVolume(this.GameProcess);
                    if (gameAudioVolume * 100 != GameAudioControlSlider.Value)
                    {
                        GameAudio.SetGameProcessAudioVolume(
                            this.GameProcess, (float)(GameAudioControlSlider.Value / 100));
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
