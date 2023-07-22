namespace ThLaunchSite.Game
{
    internal class GamePath
    {
        public static string? Th06 { get; set; }

        public static string? Th07 { get; set; }

        public static string? Th08 { get; set; }

        public static string? Th09 { get; set; }

        public static string? Th10 { get; set; }

        public static string? Th11 { get; set; }

        public static string? Th12 { get; set; }

        public static string? Th13 { get; set; }

        public static string? Th14 { get; set; }

        public static string? Th15 { get; set; }

        public static string? Th16 { get; set; }

        public static string? Th17 { get; set; }

        public static string? Th18 { get; set; }

        public static string? GetGamePath(string gameId)
        {
            if (gameId == GameIndex.Th06)
            {
                return Th06;
            }
            else if (gameId == GameIndex.Th07)
            {
                return Th07;
            }
            else if (gameId == GameIndex.Th08)
            {
                return Th08;
            }
            else if (gameId == GameIndex.Th09)
            {
                return Th09;
            }
            else if (gameId == GameIndex.Th10)
            {
                return Th10;
            }
            else if (gameId == GameIndex.Th11)
            {
                return Th11;
            }
            else if (gameId == GameIndex.Th12)
            {
                return Th12;
            }
            else if (gameId == GameIndex.Th13)
            {
                return Th13;
            }
            else if (gameId == GameIndex.Th14)
            {
                return Th14;
            }
            else if (gameId == GameIndex.Th15)
            {
                return Th15;
            }
            else if (gameId == GameIndex.Th16)
            {
                return Th16;
            }
            else if (gameId == GameIndex.Th17)
            {
                return Th17;
            }
            else if (gameId == GameIndex.Th18)
            {
                return Th18;
            }
            else
            {
                return null;
            }
        }
    }
}
