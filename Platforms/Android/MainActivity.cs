using Android.App;
using Android.Content.PM;
//using Android.Gms.Ads;
using Android.OS;
using Plugin.MauiMTAdmob;

namespace SudokuApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CrossMauiMTAdmob.Current.Init(this, "ca-app-pub-6479256761216523~8215022008");
            //MobileAds.Initialize(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            CrossMauiMTAdmob.Current.OnResume();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}

