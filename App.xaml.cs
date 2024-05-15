using Plugin.MauiMTAdmob;
using SudokuApp.Services;
using SudokuApp.ViewModels;


namespace SudokuApp
{
    public partial class App : Application
    {
        
        public App()
        {
            AddSettingsVariable();
            InitializeComponent();

            _ = Task.Run(() =>
            {
                UserAppTheme = Preferences.Default.Get("UserAppTheme", "") switch
                {
                    "Light" => AppTheme.Light,
                    "Dark" => AppTheme.Dark,
                    _ => AppTheme.Light,
                };
            });

            MainPage = new AppShell();
            BindingContext = new SoundViewModel();
        }

        private static void AddSettingsVariable()
        {
            _ = Task.Run(() =>
            {
                if (!Preferences.ContainsKey("SetCountHintTime"))
                    Preferences.Default.Set("SetCountHintTime", 0);

                if (!Preferences.Default.ContainsKey("UserAppTheme"))
                    Preferences.Default.Set("UserAppTheme", "Light");

                if (!Preferences.Default.ContainsKey("UserSound"))
                    Preferences.Default.Set("UserSound", "False");

                if (!Preferences.Default.ContainsKey("UserVibrations"))
                    Preferences.Default.Set("UserVibrations", "False");

                if (!Preferences.Default.ContainsKey("HighlightIdenticalNumber"))
                    Preferences.Default.Set("HighlightIdenticalNumber", "True");

                if (!Preferences.Default.ContainsKey("HighlightAreas"))
                    Preferences.Default.Set("HighlightAreas", "True");

                if (!Preferences.Default.ContainsKey("TimerSwitch"))
                    Preferences.Default.Set("TimerSwitch", "True");

                if (!Preferences.Default.ContainsKey("MistakesSwitch"))
                    Preferences.Default.Set("MistakesSwitch", "True");

                if (!Preferences.Default.ContainsKey("CurrentLanguage"))
                    Preferences.Default.Set("CurrentLanguage", "english");

                //CrossMauiMTAdmob.Current.UserPersonalizedAds = true;
                CrossMauiMTAdmob.Current.ComplyWithFamilyPolicies = true;
                CrossMauiMTAdmob.Current.UseRestrictedDataProcessing = true;
                CrossMauiMTAdmob.Current.AdsId = "ca-app-pub-3940256099942544/6300978111";

                if (!Preferences.Default.ContainsKey("AdsGoogleBanner"))
                    Preferences.Default.Set("AdsGoogleBanner",
                                            "ca-app-pub-3940256099942544/6300978111");

                if (!Preferences.Default.ContainsKey("AdsGoogleInterstitial"))
                    Preferences.Default.Set("AdsGoogleInterstitial",
                                            "ca-app-pub-3940256099942544/1033173712");

                CrossMauiMTAdmob.Current.TestDevices = [];
                //testowa: ca-app-pub-3940256099942544/6300978111  produkcyjna: ca-app-pub-6479256761216523/8121462788
                //Preferences.Default.Set("SetStartAdsInterstitial", 0); //obejrzyj reklamę tylko raz przy uruchiomieniu app

                //dodaj plik ze statystykami
                SaveUserStatistics saveUserStatistics = new();
                //saveUserStatistics.DeleteUserStatisticsFile();
                if (!saveUserStatistics.ExistUserStatisticsFile())
                    saveUserStatistics.CreateUserStatisticsFile();
                //dodaj plik ze statystykami
            });
        }
    }
}

