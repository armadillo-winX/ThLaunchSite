﻿using System.Xml;

namespace ThLaunchSite
{
    /// <summary>
    /// ManageEnternalToolsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ManageExternalToolsDialog : Window
    {
        public ManageExternalToolsDialog()
        {
            InitializeComponent();

            if (!File.Exists(PathInfo.ExternalToolsConfig))
            {
                try
                {
                    ExternalTool.CreateExternalConfigFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"{Properties.Resources.ErrorMessageFailedToCreateExternalToolsConfigFile}\n\n[Detail]\n{ex.Message}",
                        Properties.Resources.TitleError,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            try
            {
                GetExternalTools();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetExternalTools()
        {
            ExternalToolsListBox.Items.Clear();

            string exToolsConfig = PathInfo.ExternalToolsConfig;
            if (File.Exists(exToolsConfig))
            {
                XmlDocument exToolsConfigXml = new();
                exToolsConfigXml.Load(exToolsConfig);
                XmlNodeList exToolsNodeList = exToolsConfigXml.SelectNodes("ExternalTools/ExternalTool");
                if (exToolsNodeList.Count > 0)
                {
                    foreach (XmlNode toolNode in exToolsNodeList)
                    {
                        string name = toolNode.SelectSingleNode("Name").InnerText;
                        ExternalToolsListBox.Items.Add(name);
                    }
                }
            }
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddToolButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                AddExternalToolDialog addExternalToolDialog = new()
                {
                    Owner = this
                };
                if (addExternalToolDialog.ShowDialog() == true)
                {
                    GetExternalTools();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (ExternalToolsListBox.SelectedIndex > -1)
            {
                try
                {
                    string toolName = ExternalToolsListBox.SelectedItem.ToString();
                    ExternalTool.DeleteExternalTool(toolName);
                    GetExternalTools();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
