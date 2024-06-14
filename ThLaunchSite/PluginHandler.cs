﻿using System.Collections.Generic;
using System.Reflection;

namespace ThLaunchSite
{
    internal class PluginHandler
    {
        public static List<dynamic>? GameFilesPlugins { get; set; }

        public static List<dynamic>? GameScoreFilesPlugins { get; set; }

        public static List<dynamic>? GameProcessPlugins { get; set; }

        public static List<dynamic>? SelectedGamePlugins { get; set; }

        public static List<dynamic>? GamePlayLogPlugins { get; set; }

        public static List<dynamic>? ToolPlugins { get; set; }

        public static void GetPlugin()
        {
            GameFilesPlugins = [];
            GameScoreFilesPlugins = [];
            GameProcessPlugins = [];
            SelectedGamePlugins = [];
            GamePlayLogPlugins = [];
            ToolPlugins = [];

            string[] pluginFiles = Directory.GetFiles(PathInfo.PluginDirectory, "*.dll", SearchOption.AllDirectories);

            foreach (string pluginFile in pluginFiles)
            {
                try
                {
                    Assembly? dllAssembly = Assembly.LoadFrom(pluginFile);

                    if (dllAssembly != null)
                    {
                        ClassifyPlugin(dllAssembly);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private static void ClassifyPlugin(Assembly dllAssembly)
        {
            //dllAssemblyの中のTypeをすべて取得し、プラグインのタイプがあるかチェックする
            foreach (Type type in dllAssembly.GetTypes())
            {
                //対象のクラスのインスタンスを作成
                dynamic plugin = Activator.CreateInstance(type);

                if (type.BaseType == typeof(GameFilesPluginBase))
                {
                    GameFilesPlugins.Add(plugin);
                }
                else if (type.BaseType == typeof(GameScoreFilesPluginBase))
                {
                    GameScoreFilesPlugins.Add(plugin);
                }
                else if (type.BaseType == typeof(GameProcessPluginBase))
                {
                    GameProcessPlugins.Add(plugin);
                }
                else if (type.BaseType == typeof(SelectedGamePluginBase))
                {
                    SelectedGamePlugins.Add(plugin);
                }
                else if (type.BaseType == typeof(GamePlayLogPluginBase))
                {
                    GamePlayLogPlugins.Add(plugin);
                }
                else if (type.BaseType == typeof(ToolPluginBase))
                {
                    ToolPlugins.Add(plugin);
                }
            }
        }
    }
}
