namespace ThLaunchSite
{
    /// <summary>
    /// GameRunningTimeStaticsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GameRunningTimeStaticsDialog : Window
    {
        public string? TotalGameRunningTime
        {
            set 
            { 
                GameRunningTimeBlock.Text = value;
            }
        }
        public GameRunningTimeStaticsDialog()
        {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
