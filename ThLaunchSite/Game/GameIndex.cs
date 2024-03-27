using System.Collections.Generic;

namespace ThLaunchSite.Game
{
    internal class GameIndex
    {
        public static string Th06 => "Th06";

        public static string Th07 => "Th07";

        public static string Th08 => "Th08";

        public static string Th09 => "Th09";

        public static string Th095 => "Th095";

        public static string Th10 => "Th10";

        public static string Th11 => "Th11";

        public static string Th12 => "Th12";

        public static string Th125 => "Th125";

        public static string Th128 => "Th128";

        public static string Th13 => "Th13";

        public static string Th14 => "Th14";

        public static string Th143 => "Th143";

        public static string Th15 => "Th15";

        public static string Th16 => "Th16";

        public static string Th165 => "Th165";

        public static string Th17 => "Th17";

        public static string Th18 => "Th18";

        public static string Th185 => "Th185";

        public static string Th19 => "Th19";

        private readonly static Dictionary<string, string> _gameNameDictionary = new()
        {
            { "Th06", "東方紅魔郷" },
            { "Th07", "東方妖々夢" },
            { "Th08", "東方永夜抄" },
            { "Th09", "東方花映塚" },
            { "Th095", "東方文花帖" },
            { "Th10", "東方風神録" },
            { "Th11", "東方地霊殿" },
            { "Th12", "東方星蓮船" },
            { "Th125", "ダブルスポイラー" },
            { "Th128", "妖精大戦争" },
            { "Th13", "東方神霊廟" },
            { "Th14", "東方輝針城" },
            { "Th143", "弾幕アマノジャク" },
            { "Th15", "東方紺珠伝" },
            { "Th16", "東方天空璋" },
            { "Th165", "秘封ナイトメアダイアリー" },
            { "Th17", "東方鬼形獣" },
            { "Th18", "東方虹龍洞" },
            { "Th185", "バレットフィリア達の闇市場" },
            { "Th19", "東方獣王園" }
        };

        public static string GetGameName(string gameId)
        {
            return _gameNameDictionary[gameId];
        }
    }
}
