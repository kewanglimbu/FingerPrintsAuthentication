using FingerPrintsAuthentication.Views;

namespace FingerPrintsAuthentication
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("mainpage", typeof(MainPage));
            Routing.RegisterRoute("loginpage", typeof(LoginPage));
        }
    }
}
