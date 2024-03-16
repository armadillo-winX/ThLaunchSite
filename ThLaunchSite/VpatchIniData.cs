namespace ThLaunchSite
{
    internal class VpatchIniData
    {
        public bool IsWindowSelectionEnabled { get; set; }

        public bool IsWindowOperationEnabled { get; set; }

        public int WidnowPositionX { get; set; }

        public int WidnowPositionY { get; set; }

        public int WindowWidth { get; set; }

        public int WindowHeight { get; set; }

        public bool IsTitleBarHidden { get; set; }

        public bool IsWindowAlwaysOnTop { get; set; }

        public int VsyncType { get; set; }

        public int SleepType { get; set; }

        public int BltPrepareTime { get; set; }

        public bool IsAutoBltPrepareTime { get; set; }

        public int GameFps { get; set; }

        public int ReplaySkipFps { get; set; }

        public int ReplaySlowFps { get; set; }

        public bool IsFpsCalculationEnabled { get; set; }

        public bool IsAlwaysBlt { get; set; }

        public bool IsFixCherryBugEnabled { get; set; }

        public bool IsFixBugMarisaEnabled { get; set; }
    }
}
