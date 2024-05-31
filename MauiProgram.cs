using CommunityToolkit.Maui;
using FingerPrintsAuthentication.Services;
using FingerPrintsAuthentication.ViewModels;
using FingerPrintsAuthentication.Views;
using Microsoft.Extensions.Logging;

namespace FingerPrintsAuthentication
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddSingleton<LoginPage>();


#if IOS
            builder.Services.AddSingleton<IFingerprintSetupService, Platforms.iOS.FingerprintSetupService>();

#elif ANDROID
            builder.Services.AddSingleton<IFingerprintSetupService, Platforms.Android.FingerprintSetupService>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
