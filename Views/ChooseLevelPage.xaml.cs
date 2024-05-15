using SudokuApp.ViewModels;
using Mopups.Services;
using Plugin.MauiMTAdmob.Controls;

namespace SudokuApp.Views;

public partial class ChooseLevelPage : Mopups.Pages.PopupPage
{
    private MTAdView AdsBanner { get; set; }
    //private Grid sudokuBoardXaml { get; set; }

    public ChooseLevelPage(MTAdView adsBanner)
	{
		InitializeComponent();
        //sudokuBoardXaml = views;
        AdsBanner = adsBanner;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Utwórz pe³n¹ œcie¿kê dostêpu do pliku

        TranslationManager translate = new();
        string language = Preferences.Default.Get("CurrentLanguage", "polski");
        lblLevel1.Text = translate.Translate(language, "LabelLevel1");
        lblLevel2.Text = translate.Translate(language, "LabelLevel2");
        lblLevel3.Text = translate.Translate(language, "LabelLevel3");
        lblLevel4.Text = translate.Translate(language, "LabelLevel4");
        lblCancel.Text = translate.Translate(language, "LabelCancel");

        string filePath = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "usersudoku.json");
        if (File.Exists(filePath))
        {
            fileExistFrame.IsVisible = true;
            lblRestart.Text = translate.Translate(language, "LabelRestart"); ;
        }
    }

    private async void Frame_Tapped(object sender, TappedEventArgs e)
    {
		if(sender is Frame frame)
		{
            int number = int.Parse(frame.ClassId);

            if (number < 6)
            {
                await MopupService.Instance.PushAsync(new ActivityIndycatorPage());
                switch (frame.ClassId)
			    {
				    case "1":
                        Preferences.Default.Set("GameLevelName", "Easy");
                        break;
                    case "2":
                        Preferences.Default.Set("GameLevelName", "Medium");
                        break;
                    case "3":
                        Preferences.Default.Set("GameLevelName", "Hard");
                        break;
                    case "4":
                        Preferences.Default.Set("GameLevelName", "Expert");
                        break;
                }
                if(number < 5)
                {
                    await Navigation.PushModalAsync(new GamePage(number, true, AdsBanner), true);                
                }
                else if(number == 5) 
                {
                    Preferences.Default.Set("OpenSettings", "False");
                    await Navigation.PushModalAsync(new GamePage(number, false, AdsBanner), true);
                }
            }

            while (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            };
        }
    }

    private async void PopupPage_BackgroundClicked(object sender, EventArgs e)
    {
        if (MopupService.Instance.PopupStack.Count > 0)
        {
            await MopupService.Instance.PopAsync();
        };
    }
}