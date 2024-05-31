using Android.Provider;
using Android.Content;
using Application = Android.App.Application;
using FingerPrintsAuthentication.Services;

namespace FingerPrintsAuthentication.Platforms.Android
{
    public class FingerprintSetupService : IFingerprintSetupService
    {
        public void DoFingerprintSetup()
        {
            var intent = new Intent(Settings.ActionBiometricEnroll);
            intent.AddFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }
    }
}

