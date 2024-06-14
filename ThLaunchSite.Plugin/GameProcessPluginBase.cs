﻿namespace ThLaunchSite.Plugin
{
    public abstract class GameProcessPluginBase
    {
        /// <summary>
        /// プラグインの名前
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// プラグインのバージョン
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// プラグイン開発者名
        /// </summary>
        public abstract string Developer { get; }

        /// <summary>
        /// プラグインの説明
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// プラグイン呼び出し時に実行される関数
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="gameProcessName"></param>
        public abstract void Main(string gameId, string gameProcessName);
    }
}
