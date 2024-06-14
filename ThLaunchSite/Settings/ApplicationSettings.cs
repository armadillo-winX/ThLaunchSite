namespace ThLaunchSite.Settings
{
    public class ApplicationSettings
    {
        public double MainWindowWidth { get; set; }

        public double MainWindowHeight { get; set; }

        public bool FixMainWindowSize { get; set; }

        public string? SelectedGameId { get; set; }

        public int SelectedGamePriorityIndex { get; set; }

        public bool AlwaysOnTop { get; set; }

        public bool AutoBackup { get; set; }

        public int ResizeRateIndex { get; set; }

        public bool ResizeByRate { get; set; }

        public bool ResizeBySize { get; set; }

        public string? ResizeWidth { get; set; }

        public string? ResizeHeight { get; set; }

        public string? ThemeName { get; set; }

        public string? LanguageId { get; set; }

        public string? CaptureFileDirectory { get; set; }

        public string? CaptureFileFormat { get; set; }
    }
}
