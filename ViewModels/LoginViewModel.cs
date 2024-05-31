using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FingerPrintsAuthentication.Services;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace FingerPrintsAuthentication.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        public string _Email;

        [ObservableProperty]
        public string _Password;

        private readonly IFingerprintSetupService _fingerprintSetupService;
        private int _failedAttempts;
        private DateTime _lastFailedAttemptTime;
        private static readonly TimeSpan _lockoutDuration = TimeSpan.FromSeconds(30);

        public LoginViewModel(IFingerprintSetupService fingerprintSetupService)
        {
            _fingerprintSetupService = fingerprintSetupService;
            _failedAttempts = 0;
            _lastFailedAttemptTime = DateTime.MinValue;
        }

        [RelayCommand]
        public async Task OnLoginWithBiometricClicked()
        {
            if (DateTime.Now - _lastFailedAttemptTime < _lockoutDuration)
            {
                var remainingLockoutTime = _lockoutDuration - (DateTime.Now - _lastFailedAttemptTime);
                await ShowLockoutMessage(remainingLockoutTime);
                return;
            }

            FingerprintAvailability availability;

            if (_failedAttempts == 0)
            {
                availability = await CrossFingerprint.Current.GetAvailabilityAsync();
            }
            else
            {
                availability = FingerprintAvailability.Available;
            }

            if (availability == FingerprintAvailability.Available)
            {
                var request = new AuthenticationRequestConfiguration("Place your finger to login", "");
                var result = await CrossFingerprint.Current.AuthenticateAsync(request);

                if (result.Authenticated)
                {
                    _failedAttempts = 0;
                    _lastFailedAttemptTime = DateTime.MinValue;
                    var cancellationTokenSource = new CancellationTokenSource();
                    await Toast.Make("Login Successfully", ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
                    await Shell.Current.GoToAsync("mainpage");
                }
                else if (result.Status == FingerprintAuthenticationResultStatus.TooManyAttempts)
                {
                    var cancellationTokenSource = new CancellationTokenSource();
                    await Toast.Make("Too many attempts. Please try again later.", ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
                    HandleFailedAttempt();
                }
                else if (result.Status == FingerprintAuthenticationResultStatus.Canceled)
                {
                    var cancellationTokenSource = new CancellationTokenSource();
                    await Toast.Make("Cancel", ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
                }
                else if(result.Status == FingerprintAuthenticationResultStatus.Unknown)
                {
                    var cancellationTokenSource = new CancellationTokenSource();
                    await Toast.Make("Something went wrong !", ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
                }
            }
            else
            {
                await HandleFingerprintNotAvailable(availability);
            }
        }

        private void HandleFailedAttempt()
        {
            _failedAttempts++;
            _lastFailedAttemptTime = DateTime.Now;
        }

        private async Task ShowLockoutMessage(TimeSpan remainingLockoutTime)
        {
            string message = $"Too many attempts. Please try again in {remainingLockoutTime.Minutes} minutes and {remainingLockoutTime.Seconds} second.";
            await Application.Current.MainPage.DisplayAlert("Fingerprint Lockout", message, "OK");
        }

        private async Task HandleFingerprintNotAvailable(FingerprintAvailability availability)
        {
            string message = availability switch
            {
                FingerprintAvailability.NoFingerprint => "No fingerprint is registered. Please set up a fingerprint.",
                FingerprintAvailability.NoPermission => "No permission to use fingerprint. Please enable fingerprint permissions in settings.",
                _ => "Fingerprint authentication is not available on this device."
            };

            bool setupFingerprint = await Application.Current.MainPage.DisplayAlert("Fingerprint Unavailable", message, "Set Up Fingerprint", "Cancel");

            if (setupFingerprint)
            {
                _fingerprintSetupService.DoFingerprintSetup();
            }
        }
    }
}
