using System.Windows;

namespace WebApiDemoClient.Authenticate.Window
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogIn
    {
        public LogIn(LoginWindowViewModel loginWindowViewModel)
        {
            InitializeComponent();
            DataContext = loginWindowViewModel;
        }

        private void BtnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
