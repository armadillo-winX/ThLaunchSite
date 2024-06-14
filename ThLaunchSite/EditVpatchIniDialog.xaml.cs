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
                    EditorBox.Text = $"{Properties.Resources.ErrorMessageFailedToReadVpatchIni}\n{ex.Message}";
                    SaveButton.IsEnabled = false;
                }
            }
            else
            {
                EditorBox.IsReadOnly = true;
                EditorBox.Text = Properties.Resources.ErrorMessageNotFoundVpatchIni;
                SaveButton.IsEnabled = false;
            }
        }

        private void SaveVsyncPatchIniData(string data)
        {
            File.WriteAllText(this.VsyncPatchIniFilePath, data);
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveVsyncPatchIniData(EditorBox.Text);
                MessageBox.Show(this, Properties.Resources.MessageSave, Properties.Resources.TitleVpatchIniEditor,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"{Properties.Resources.ErrorMessageFailedToSaveFile}\n{ex.Message}", Properties.Resources.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Close();
            }
        }
    }
}
