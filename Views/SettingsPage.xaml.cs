using SudokuApp.Services.Audio;
using Plugin.MauiMTAdmob;
using Plugin.MauiMTAdmob.Controls;
using Mopups.Services;
using SudokuApp.ViewModels;

namespace SudokuApp.Views;

public partial class SettingsPage : ContentPage
{
    private string AdsInterstitial { get; set; }

    public SettingsPage(MTAdView adsBanner)
	{
        InitializeComponent();
        _ = Task.Run(() =>
        {
            StackReklama.Children.Add(adsBanner);
            //StackReklama.HeightRequest = adsBanner.Height;
        });
        _ = Task.Run(() =>
        {
            AdsInterstitial = Preferences.Default.Get("AdsGoogleInterstitial", "");
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _ = Task.Run(() =>
        {
            ThemeSwitch.IsToggled = Preferences.Default.Get("UserAppTheme", "") switch
            {
                "Dark" => true,
                "Light" => false,
                _ => false,
            };
            SoundSwitch.IsToggled = Preferences.Default.Get("UserSound", "") switch
            {
                "True" => true,
                "False" => false,
                _ => false,
            };
            VibrationsSwitch.IsToggled = Preferences.Default.Get("UserVibrations", "") switch
            {
                "True" => true,
                "False" => false,
                _ => false,
            };
        });

        _ = Task.Run(() =>
        {
            IdenticalNumberSwitch.IsToggled = Preferences.Default.Get("HighlightIdenticalNumber", "") switch
            {
                "True" => true,
                "False" => false,
                _ => true,
            };
            AreasSwitch.IsToggled = Preferences.Default.Get("HighlightAreas", "") switch
            {
                "True" => true,
                "False" => false,
                _ => true,
            };
            TimerSwitch.IsToggled = Preferences.Default.Get("TimerSwitch", "") switch
            {
                "True" => true,
                "False" => false,
                _ => true,
            };
            MistakesSwitch.IsToggled = Preferences.Default.Get("MistakesSwitch", "") switch
            {
                "True" => true,
                "False" => false,
                _ => true,
            };
        });
        string language = Preferences.Default.Get("CurrentLanguage", "polski");
        TranslationManager translate = new();
        foreach (View child in absoluteLayout.Children.Cast<View>())
        {
            if (child is Grid grid)
            {
                foreach (View child1 in grid.Children.Cast<View>())
                {
                    if (child1 is Label label)
                    {
                        label.Text = translate.Translate(language, label.ClassId);
                    }
                    if (child1 is Button button)
                    {
                        button.Text = translate.Translate(language, button.ClassId);
                    }
                }
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Preferences.Default.Set("OpenSettings", "True");
    }

    private async void ImageBack_Tapped(object sender, TappedEventArgs e)
    {
        // Zamknij bie¿¹c¹ stronê
        TapSound.PlaySound();
        while (Navigation.ModalStack.Count > 0)
        {
            await Navigation.PopModalAsync();
        }
    }

    private void SwitchThemeChange_Toggled(object sender, ToggledEventArgs e)
    {
        switch (e.Value)
        {
            case true:
                Preferences.Default.Set("UserAppTheme", "Dark");
                Application.Current.UserAppTheme = AppTheme.Dark;
            break;
            case false:
                Preferences.Default.Set("UserAppTheme", "Light");
                Application.Current.UserAppTheme = AppTheme.Light;
            break;
        }
    }

    private void SwitchSoundChange_Toggled(object sender, ToggledEventArgs e)
    {
        switch (e.Value)
        {
            case true: Preferences.Default.Set("UserSound", "True"); break;
            case false:Preferences.Default.Set("UserSound", "False"); break;
        }
    }

    private void SwitchVibrationsChange_Toggled(object sender, ToggledEventArgs e)
    {
        switch (e.Value)
        {
            case true: Preferences.Default.Set("UserVibrations", "True"); break;
            case false: Preferences.Default.Set("UserVibrations", "False"); break;
        }
    }

    private void SwitchIdenticalNumberChange_Toggled(object sender, ToggledEventArgs e)
    {
        switch (e.Value)
        {
            case true: Preferences.Default.Set("HighlightIdenticalNumber", "True"); break;
            case false: Preferences.Default.Set("HighlightIdenticalNumber", "False"); break;
        }
    }

    private void SwitchAreasChange_Toggled(object sender, ToggledEventArgs e)
    {
        switch (e.Value)
        {
            case true: Preferences.Default.Set("HighlightAreas", "True"); break;
            case false: Preferences.Default.Set("HighlightAreas", "False"); break;
        }
    }

    private void SwitchTimerChange_Toggled(object sender, ToggledEventArgs e)
    {
        switch (e.Value)
        {
            case true: Preferences.Default.Set("TimerSwitch", "True"); break;
            case false: Preferences.Default.Set("TimerSwitch", "False"); break;
        }
    }

    private void SwitchMistakesChange_Toggled(object sender, ToggledEventArgs e)
    {
        switch (e.Value)
        {
            case true: Preferences.Default.Set("MistakesSwitch", "True"); break;
            case false:
                CrossMauiMTAdmob.Current.LoadInterstitial($"{AdsInterstitial}");
                Preferences.Default.Set("MistakesSwitch", "False"); 
                break;
        }
    }

    private async void ButtonRules_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new ActivityIndycatorPage());
        await Navigation.PushModalAsync(new GameRulesPage(), false);
        while (MopupService.Instance.PopupStack.Count > 0)
        {
            await MopupService.Instance.PopAsync();
        };
    }

    private async void ButtonLanguage_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new LanguagePage(), false);
        //await MopupService.Instance.PushAsync(new LanguagePage(), false);
    }

    private async void ButtonStatistic_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new StatisticPage(), false);
    }

    //private void MyAds_Loaded(object sender, EventArgs e) => MyAds.LoadAd();
}