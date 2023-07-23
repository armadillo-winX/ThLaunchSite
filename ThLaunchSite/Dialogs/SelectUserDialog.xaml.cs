namespace ThLaunchSite.Dialogs
{
    /// <summary>
    /// SelectUserDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectUserDialog : Window
    {
        public string? SelectedUserName { get; set; }

        private readonly string _userIndex = PathInfo.UserIndex;

        public SelectUserDialog()
        {
            InitializeComponent();

            if (File.Exists(_userIndex))
            {
                try
                {
                    XmlDocument doc = new();
                    doc.Load(_userIndex);
                    XmlNodeList userNodeList = doc.SelectNodes("UserIndex/User");
                    if (userNodeList.Count > 0)
                    {
                        foreach (XmlNode userNode in userNodeList)
                        {
                            string userName = userNode.SelectSingleNode("Name").InnerText;
                            if (userName != User.CurrentUserName)
                                _ = UsersListBox.Items.Add(userName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "エラー");
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedIndex > -1)
            {
                string userName = UsersListBox.SelectedItem.ToString();
                this.SelectedUserName = userName;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show(
                    this, "ユーザーを選択してください。", "ユーザーの選択",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
