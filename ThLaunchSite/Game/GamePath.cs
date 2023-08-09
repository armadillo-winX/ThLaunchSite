namespace ThLaunchSite.Game
{
    internal class GamePath
    {
        public static string? Th06FilePath { get; set; }

        public static string? Th07FilePath { get; set; }

        public static string? Th08FilePath { get; set; }

        public static string? Th09FilePath { get; set; }

        public static string? Th10FilePath { get; set; }

        public static string? Th11FilePath { get; set; }

        public static string? Th12FilePath { get; set; }

        public static string? Th13FilePath { get; set; }

        public static string? Th14FilePath { get; set; }

        public static string? Th15FilePath { get; set; }

        public static string? Th16FilePath { get; set; }

        public static string? Th17FilePath { get; set; }

        public static string? Th18FilePath { get; set; }

        public static string? GetGamePath(string gameId)
        {
            if (gameId == GameIndex.Th06)
            {
                return Th06FilePath;
            }
            else if (gameId == GameIndex.Th07)
            {
                return Th07FilePath;
            }
            else if (gameId == GameIndex.Th08)
            {
                return Th08FilePath;
            }
            else if (gameId == GameIndex.Th09)
            {
                return Th09FilePath;
            }
            else if (gameId == GameIndex.Th10)
            {
                return Th10FilePath;
            }
            else if (gameId == GameIndex.Th11)
            {
                return Th11FilePath;
            }
            else if (gameId == GameIndex.Th12)
            {
                return Th12FilePath;
            }
            else if (gameId == GameIndex.Th13)
            {
                return Th13FilePath;
            }
            else if (gameId == GameIndex.Th14)
            {
                return Th14FilePath;
            }
            else if (gameId == GameIndex.Th15)
            {
                return Th15FilePath;
            }
            else if (gameId == GameIndex.Th16)
            {
                return Th16FilePath;
            }
            else if (gameId == GameIndex.Th17)
            {
                return Th17FilePath;
            }
            else if (gameId == GameIndex.Th18)
            {
                return Th18FilePath;
            }
            else
            {
                return null;
            }
        }

        public static void SetGamePath(string gameId, string path)
        {
            if (gameId == GameIndex.Th06)
            {
                Th06FilePath = path;
            }
            else if (gameId == GameIndex.Th07)
            {
                Th07FilePath = path;
            }
            else if (gameId == GameIndex.Th08)
            {
                Th08FilePath = path;
            }
            else if (gameId == GameIndex.Th09)
            {
                Th09FilePath = path;
            }
            else if (gameId == GameIndex.Th10)
            {
                Th10FilePath = path;
            }
            else if (gameId == GameIndex.Th11)
            {
                Th11FilePath = path;
            }
            else if (gameId == GameIndex.Th12)
            {
                Th12FilePath = path;
            }
            else if (gameId == GameIndex.Th13)
            {
                Th13FilePath = path;
            }
            else if (gameId == GameIndex.Th14)
            {
                Th14FilePath = path;
            }
            else if (gameId == GameIndex.Th15)
            {
                Th15FilePath = path;
            }
            else if (gameId == GameIndex.Th16)
            {
                Th16FilePath = path;
            }
            else if (gameId == GameIndex.Th17)
            {
                Th17FilePath = path;
            }
            else if (gameId == GameIndex.Th18)
            {
                Th18FilePath = path;
            }
        }
    }
}
