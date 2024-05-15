using SudokuApp.ViewModels;
using Mopups.Services;

namespace SudokuApp.Views;

public partial class LanguagePage : Mopups.Pages.PopupPage
{
    private TranslationManager translate { get; set; }
    private string language;

    private readonly Color Primary = Color.FromArgb("#512BD4");
    private readonly Color MyPrimary = Color.FromArgb("#2196F3");

    public LanguagePage()
	{
        InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        translate = new();
        language = Preferences.Default.Get("CurrentLanguage", "polski");
        lblLaunguageText.Text = translate.Translate(language, "BtnTextLanguage");
        lblLaunguageConfirm.Text = translate.Translate(language, "LabelLaunguageConfirm");
        MarkButton();
    }

    private void MarkButton()
    {
        foreach (View view in StackButton.Children.Cast<View>())
        {
            if (view is Frame frame)
            {
                if (frame.Content is Button button && button.ClassId == language)
                {
                    _ = scrollView.ScrollToAsync(button, position: ScrollToPosition.Center, true);
                    frame.Background = AppTheme.Light.ToString() == "Light" ? MyPrimary : Primary;
                }
                else
                {
                    frame.Background = Color.FromArgb("#00FFFFFF");
                }
            }
        }
    }

    private void Confirm_Tapped(object sender, TappedEventArgs e)
    {
        translate.SetLanguage(language);
        Cancel_Tapped(null, null);
    }

    private async void Cancel_Tapped(object sender, TappedEventArgs e)
    {
        while (Navigation.ModalStack.Count > 0)
        {
            await Navigation.PopModalAsync();
        };
        //while (MopupService.Instance.PopupStack.Count > 0)
        //{
        //    await MopupService.Instance.PopAsync();
        //};
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            language = button.ClassId switch
            {
                "cesky" => button.ClassId,
                "dansk" => button.ClassId,
                "deutsch" => button.ClassId,
                "english" => button.ClassId,
                "espanol" => button.ClassId,
                "francais" => button.ClassId,
                "italiano" => button.ClassId,
                "nederlands" => button.ClassId,
                "norsk" => button.ClassId,
                "polski" => button.ClassId,
                "portuges" => button.ClassId,
                _ => Preferences.Default.Get("CurrentLanguage", "polski")
            };
            MarkButton();
        }
    }
}