using FingerPrintsAuthentication.Services;
using Foundation;
using UIKit;

namespace FingerPrintsAuthentication.Platforms.iOS
{
    public class FingerprintSetupService : IFingerprintSetupService
    {
        public void DoFingerprintSetup()
        {
            var url = new NSUrl("App-Prefs:root=TOUCHID_PASSCODE");
            if (UIApplication.SharedApplication.CanOpenUrl(url))
            {
                UIApplication.SharedApplication.OpenUrl(url);
            }
        }
    }
}
