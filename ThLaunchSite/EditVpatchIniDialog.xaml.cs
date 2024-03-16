using System.Text;

namespace ThLaunchSite
{
    /// <summary>
    /// EditVpatchIniDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class EditVpatchIniDialog : Window
    {
        public EditVpatchIniDialog()
        {
            InitializeComponent();
        }

        private string? _vsyncPatchIniFilePath;

        public string? VsyncPatchIniFilePath 
        {
            get
            {
                return _vsyncPatchIniFilePath;
            }

            set
            {
                _vsyncPatchIniFilePath = value;
                GetVsyncPatchIniData(value);
            }
        }

        private void GetVsyncPatchIniData(string vsyncPatchIniFilePath)
        {
            if (File.Exists(vsyncPatchIniFilePath))
            {
                try
                {
                    StreamReader streamReader = new(vsyncPatchIniFilePath, Encoding.UTF8);
                    string data = streamReader.ReadToEnd();
                    streamReader.Close();

                    EditorBox.Text = data;
                }
                catch (Exception ex)
                {
                    EditorBox.IsReadOnly = true;
                    EditorBox.Text = $"vpatch.iniの読み込みに失敗。\n{ex.Message}";
                    SaveButton.IsEnabled = false;
                }
            }
            else
            {
                EditorBox.IsReadOnly = true;
                EditorBox.Text = "vpatch.iniが見つかりませんでした。";
                SaveButton.IsEnabled = false;
            }
        }

        private void SaveVsyncPatchIniData(string data)
        {
            File.WriteAllText(data, this.VsyncPatchIniFilePath);
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveVsyncPatchIniData(EditorBox.Text);
                MessageBox.Show(this, "保存しました。", "ファイルの保存",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ファイルの保存に失敗。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Close();
            }
        }
    }
}
