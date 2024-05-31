using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace FingerPrintsAuthentication.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            Dispatcher.Dispatch(async () =>
            {
                var exit = await DisplayAlert("", "Are you sure you want to logout?", "Yes", "No");

                if (exit)
                {
                    //Application.Current.Quit();
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                    var cancellationTokenSource = new CancellationTokenSource();
                    await Toast.Make("Logged out Successfully", ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
                }
            });

            return true;
        }
    }
}

