using Plugin.MauiMTAdmob;
using SudokuApp.Views;
using Mopups.Services;
using SudokuApp.Services;
using SudokuApp.ViewModels;

namespace SudokuApp
{
    public partial class MainPage : ContentPage
    {
        private CountTimerDown TimerProperty { get; set; }
        //private string AdsInterstitial { get; set; }
        //private string AdsBannerId { get; set; }
        //private MTAdView AdsBanner { get; set; }

        public MainPage()
        {
            _ = Task.Run(() =>
            {
                //AdsInterstitial = Preferences.Default.Get("AdsGoogleInterstitial", "");
                //AdsBannerId = Preferences.Default.Get("AdsGoogleBanner", "");
                //CrossMauiMTAdmob.Current.OnInterstitialLoaded += (s, args) =>
                //{
                //    CrossMauiMTAdmob.Current.ShowInterstitial();
                //};
                int counter = Preferences.Default.Get("SetCountHintTime", 0);
                if (counter > 0)
                {
                    TimerProperty = new CountTimerDown(counter);
                    TimerProperty.CountdownTick += OnCountdownTick; // Set the countup time in seconds
                    TimerProperty?.Start();
                }
            });
            InitializeComponent();
        }

        private void MyAds_Loaded(object sender, EventArgs e) => AdsBanner.LoadAd();

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Preferences.Default.Set("OpenSettings", "False");
            Preferences.Default.Set("MistakesSwitch", "True"); //default activated

            //if (Preferences.Default.Get("SetStartAdsInterstitial", 0) == 0)
            //{
            //    _ = Task.Run(LoadInterstitial);
            //}

            MainThread.BeginInvokeOnMainThread(() => //MainThread.BeginInvokeOnMainThread
            {
                string language = Preferences.Default.Get("CurrentLanguage", "polski");
                var translate = new TranslationManager();

                lblNewGame.Text = translate.Translate(language, "LabelNewGame");

                if (Preferences.Default.ContainsKey("SetCountTime"))
                {
                    int countedSeconds = Preferences.Default.Get("SetCountTime", 0);

                    if (countedSeconds > 0)
                    {
                        // Convert seconds to hours, minutes, and seconds
                        //int hours = countedSeconds / 3600;
                        int minutes = countedSeconds % 3600 / 60;
                        int seconds = countedSeconds % 60;

                        lblTime.Text = string.Format(" {0:D2}:{1:D2}", minutes, seconds);
                        //var labelLevel = Preferences.Default.Get("GameLevelName", "") switch
                        //{
                        //    "Łatwy" => "LabelLevel1",
                        //    "Średni" => "LabelLevel2",
                        //    "Trudny" => "LabelLevel3",
                        //    "Ekspert" => "LabelLevel4",
                        //    _ => throw new NotImplementedException()
                        //};

                        string labelLevel = Preferences.Default.Get("GameLevelName", "") switch
                        {
                            "Easy" => "LabelLevel1",
                            "Medium" => "LabelLevel2",
                            "Hard" => "LabelLevel3",
                            "Expert" => "LabelLevel4",
                            _ => ""
                        };
                        lblTime.Text += $" - {translate.Translate(language, labelLevel)}";
                        lblContinue.Text = translate.Translate(language, "LabelContinue");
                        frameContinue.IsVisible = true;
                    }
                    else
                    {
                        frameContinue.IsVisible = false;
                    }
                }
            });
        }

        //private void LoadInterstitial()
        //{
        //    try
        //    {
        //        _ = MainThread.InvokeOnMainThreadAsync(() =>
        //        {
        //            CrossMauiMTAdmob.Current.LoadInterstitial(AdsInterstitial);
        //            Preferences.Default.Set("SetStartAdsInterstitial", 1);
        //        });
        //    }
        //    catch (Exception ex) { Console.WriteLine(ex.Message); }
        //}

        private async void Continue_Tapped(object sender, EventArgs e)
        {
            await MopupService.Instance.PushAsync(new ActivityIndycatorPage());

            await Navigation.PushModalAsync(new GamePage(6, false, AdsBanner), true);

            while (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            };
        }

        private async void ChooseLevel_Tapped(object sender, EventArgs e)
        {
            var popup = new ChooseLevelPage(AdsBanner);
            await MopupService.Instance.PushAsync(popup, true);
        }

        private async void ImageSettings_Tapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushModalAsync(new SettingsPage(AdsBanner), true);
        }

        private void OnCountdownTick(object sender, int countedSeconds)
        {
            _ = Task.Run(() =>
            {
                if (countedSeconds > 0)
                {
                    Preferences.Default.Set("SetCountHintTime", countedSeconds);
                }
                else
                {
                    Preferences.Default.Set("SetCountHintTime", 0);
                    TimerProperty?.Stop();
                }
            });
        }

    }
}
