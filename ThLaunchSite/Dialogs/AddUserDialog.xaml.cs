namespace ThLaunchSite.Dialogs
{
    /// <summary>
    /// AddUserDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AddUserDialog : Window
    {
        public string? UserName { get; set; }

        public AddUserDialog()
        {
            InitializeComponent();
            _ = UserNameBox.Focus();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserNameBox.Text;
            if (!String.IsNullOrEmpty(userName))
            {
                try
                {
                    if (!User.Exist(userName))
                    {
                        User.AddUser(userName);
                        this.UserName = userName;
                        DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show(
                            this, $"このユーザーはすでに存在します。", "ユーザーの追加",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        _ = UserNameBox.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(
                    this, $"ユーザー名を入力してください", "ユーザーの追加",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                _ = UserNameBox.Focus();
            }
        }
    }
}
