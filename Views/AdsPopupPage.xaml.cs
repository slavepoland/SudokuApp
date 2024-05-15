using Mopups.Services;
using Plugin.MauiMTAdmob;
using Plugin.MauiMTAdmob.Controls;
using SudokuApp.Services;
using SudokuApp.ViewModels;


namespace SudokuApp
{
    public partial class AdsPopupPage : Mopups.Pages.PopupPage
    {
        private readonly int CountDownTime = 120;
        private CountTimerDown TimerProperty { get; set; }
        private string AdsInterstitial { get; set; }
        private bool isInterstitialLoaded = false;
        //private string AdsBannerId { get; set; }
        //private MTAdView AdsBanner { get; set; }

        private TranslationManager translate { get; set; }
        private string language;

        public AdsPopupPage()
        {
            AdsInterstitial = Preferences.Default.Get("AdsGoogleInterstitial", "");
            _ = Task.Run(LoadInterstitial);
            
            _ = Dispatcher.Dispatch(() =>
            {
                int counter = Preferences.Default.Get("SetCountHintTime", 0);
                if (counter > 0)
                {
                    TimerProperty = new CountTimerDown(counter); // Set the countdown time in seconds
                    btnAdsWatch.IsEnabled = false;
                    TimerProperty.CountdownTick += OnCountdownTick;
                    TimerProperty.Start();
                }
            });

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = Task.Run(() =>
            {
                translate = new TranslationManager();
                language = Preferences.Default.Get("CurrentLanguage", "polski");
                lblGetHint.Text = translate.Translate(language, "LabelGetTips");
                lblSeeTips.Text = translate.Translate(language, "LabelSeeTips");
                btnAdsWatch.Text = translate.Translate(language, "BtnSeeAd");
            });   
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Preferences.Default.Set("OpenSettings", "True");
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            // Zamknij bie¿¹c¹ stronê
            while (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            MainAdsStack.IsEnabled = false;
            btnAdsWatch.IsEnabled = false;
            TimerProperty = new CountTimerDown(CountDownTime); // Set the countdown time in seconds
            TimerProperty.CountdownTick += OnCountdownTick;
            TimerProperty.Start();

            _ = Task.Run(ShowInterstitialIfLoaded);

            //Preferences.Default.Set("HintAvilable", Preferences.Default.Get("HintAvilable", 0) + 1);
        }

        private void ShowInterstitialIfLoaded()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (isInterstitialLoaded)
                    {
                        CrossMauiMTAdmob.Current.ShowInterstitial();
                        isInterstitialLoaded = false; // Ustawiamy na false, aby mo¿na by³o ponownie za³adowaæ reklamê
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        MainAdsStack.IsEnabled = true;
                        _ = Task.Run(LoadInterstitial); // Wczytujemy kolejn¹ reklamê po wyœwietleniu poprzedniej
                        Preferences.Default.Set("HintAvilable", Preferences.Default.Get("HintAvilable", 0) + 1);
                    }
                    else
                    {
                        MainAdsStack.IsEnabled = true;
                        _ = Task.Run(LoadInterstitial);
                    }
                }); 
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void LoadInterstitial()
        {
            try
            {
                _ = MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    CrossMauiMTAdmob.Current.OnInterstitialLoaded += (s, args) =>
                    {
                        isInterstitialLoaded = true;
                    };
                    
                    CrossMauiMTAdmob.Current.OnInterstitialFailedToLoad += (s, args) =>
                    {
                        MainAdsStack.IsEnabled = true;
                        _ = Task.Run(LoadInterstitial);
                    };

                    CrossMauiMTAdmob.Current.OnInterstitialFailedToShow += (s, args) =>
                    {
                        MainAdsStack.IsEnabled = true;
                        _ = Task.Run(LoadInterstitial);
                    };

                    CrossMauiMTAdmob.Current.LoadInterstitial($"{AdsInterstitial}");

                    // Poczekaj na za³adowanie reklamy
                    while (!CrossMauiMTAdmob.Current.IsInterstitialLoaded())
                    {
                        await Task.Delay(50); // Poczekaj 50 ms przed sprawdzeniem ponownie
                    }
                });
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void MyAds_Loaded(object sender, EventArgs e) => AdsBanner.LoadAd();

        private void OnCountdownTick(object sender, int remainingSeconds)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                // Convert remaining seconds to hours, minutes, and seconds
                //int hours = remainingSeconds / 3600;
                int minutes = remainingSeconds % 3600 / 60;
                int seconds = remainingSeconds % 60;
                try
                {
                    btnAdsWatch.Text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
                    //string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
                }
                catch (Exception) { };
            });//string.Format("{0:D2}:{1:D2}", minutes, seconds)


            Preferences.Default.Set("SetCountHintTime", remainingSeconds);

            if (remainingSeconds == 0)
            {
                TimerProperty.Stop();
                MainAdsStack.IsEnabled = true;
                btnAdsWatch.IsEnabled = true;
                btnAdsWatch.Text = translate.Translate(language, "BtnSeeAd");
            }
        }
    }
}