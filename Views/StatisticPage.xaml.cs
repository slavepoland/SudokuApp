using AndroidX.AppCompat.App;
using Microsoft.Maui.Controls;
using Mopups.Services;
using Plugin.MauiMTAdmob;
using SudokuApp.Services;
using SudokuApp.Services.Audio;
using SudokuApp.ViewModels;
using System.Collections;
using System.Globalization;

namespace SudokuApp.Views;

public partial class StatisticPage : ContentPage
{
    private int currentIndex = -1;
    private string language { get; set; }
    private TranslationManager translate {  get; set; }
    private SaveUserStatistics saveUserStatistics { get; set; }
    private List<string> items {  get; set; }

    public StatisticPage()
	{
		InitializeComponent();
        language = Preferences.Default.Get("CurrentLanguage", "polski");
        translate = new();
        saveUserStatistics = new();
        myCarouselView.CurrentItemChanged += (sender, args) =>
        {
            var carouselView = (CarouselView)sender;
            var items = carouselView.ItemsSource as IList;
            currentIndex = items.IndexOf(args.CurrentItem);
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string input = translate.Translate(language, "BtnTextStatistic");
        lblStatistic.Text = input.First().ToString().ToUpper(CultureInfo.CurrentCulture) + 
            input[1..].ToString().ToLower(CultureInfo.CurrentCulture);

        btnReset.Text = translate.Translate(language, "BtnReset");

        items = ["LabelLevel1", "LabelLevel2", "LabelLevel3", "LabelLevel4"];

        for (int i = 0; i < items.Count; i++)
        {
            items[i] = translate.Translate(language, items[i]);
        }
        List<View> views = [];
        foreach (string item in items)
        {
            Label label = new()
            {
                Text = item,
            };
            views.Add(label); // Dodajemy ka¿d¹ etykietê do kolekcji views
        }
        myCarouselView.ItemsSource = views;

        _ = Task.Run(action: GetUserStatistic);

        foreach (View view in myGrid.Children.Cast<View>())
        {
            if (view is Label label)
            {
                if (label.ClassId.Contains("Label"))
                {
                    label.Text = translate.Translate(language, label.ClassId);
                }
            }
        }
    }

    private void GetUserStatistic()
    {
        string curentLevel = myCarouselView.Position switch
        {
            0 => "Easy",
            1 => "Medium",
            2 => "Hard",
            3 => "Expert",
            _ => ""
        };

        string[] getUserStatistics = saveUserStatistics.GetUserStatistics(curentLevel);
        int number = 1;

        foreach (View view in myGrid.Children.Cast<View>())
        {
            if (view is Label label)
            {
                if (label.ClassId.Contains("Number"))
                {
                    label.Text = getUserStatistics[number];

                    switch (number)
                    {
                        case 5: number += 2; break;
                        default: number++; break;
                    }
                }
            }
        }
    }

    private async void ImageBack_Tapped(object sender, TappedEventArgs e)
    {
        TapSound.PlaySound();
        while (Navigation.ModalStack.Count > 0)
        {
            await Navigation.PopModalAsync();
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var btnClicked = sender as ImageButton;
        switch(btnClicked.ClassId)
        {
            case "Left":
                if (currentIndex > 0)
                {
                    currentIndex--;
                    myCarouselView.Position = currentIndex;
                }
                break;
            case "Right":
                if (currentIndex < 3)
                {
                    currentIndex++;
                    myCarouselView.Position = currentIndex;
                }
             break;
        }
        switch (currentIndex)
        {
            case 0: 
                LeftBtn.IsEnabled = false;
                RightBtn.IsEnabled = true;
                break;
            case 3:
                LeftBtn.IsEnabled = true;
                RightBtn.IsEnabled = false;
                break;
            default:
                LeftBtn.IsEnabled = true;
                RightBtn.IsEnabled = true;
                break;
        }
        GetUserStatistic();
    }

    private void OnSwiped(object sender, SwipedEventArgs e)
    {
        switch (e.Direction)
        {
            case SwipeDirection.Left:
                Button_Clicked(RightBtn, null);
                break;
            case SwipeDirection.Right:
                Button_Clicked(LeftBtn, null);
                break;
        }
    }

    private async void BtnReset_Clicked(object sender, EventArgs e)
    {
        string translateLevelName = myCarouselView.Position switch
        {
            0 => translate.Translate(language, "LabelLevel1"),
            1 => translate.Translate(language, "LabelLevel2"),
            2 => translate.Translate(language, "LabelLevel3"),
            3 => translate.Translate(language, "LabelLevel4"),
            _ => ""
        };
        string body = translate.Translate(language, "ResetStatisticsText");

        switch (language )
        {
            case "cesky": body = body.Replace("()", $"{translateLevelName}"); break;
            case "dansk": body = $"{translateLevelName} " + body; break;
            case "deutsch" : body = $"{translateLevelName} " + body; break;
            case "english": body = $"{translateLevelName} " + body; break;
            case "espanol" : body = body.Replace("()", $"{translateLevelName}"); break;
            case "francais": body = body.Replace("()", $"{translateLevelName}"); break;
            case "italiano": body = body.Replace("()", $"{translateLevelName}"); break;
            case "nederlands": body = body.Replace("()", $"\"{translateLevelName}\""); break;
            case "norsk": body = body.Replace("()", $"{translateLevelName}"); break;
            case "polski": body = body.Replace("()", $"({translateLevelName})"); break;
            case "portuges": body = body.Replace("()", $"({translateLevelName})"); break;
        };

        string yes = translate.Translate(language, "TextYes");
        string no = translate.Translate(language, "TextNo");

        // Wyœwietlenie niestandardowego okna dialogowego jako modalnego okna
        var customDialog = new CustomDialogStatistics($"{body}", $"{yes}", $"{no}");
        await MopupService.Instance.PushAsync(customDialog, true);

        // Oczekiwanie na zamkniêcie okna dialogowego
        var result = await customDialog.ResultTask;

        //bool result = await DisplayAlert("", $"{body}", $"{yes}", $"{no}", FlowDirection.LeftToRight);
        if (result)
        {
            string curentLevel = myCarouselView.Position switch
            {
                0 => "Easy",
                1 => "Medium",
                2 => "Hard",
                3 => "Expert",
                _ => ""
            };
            saveUserStatistics.DeleteUserStatistics(curentLevel);
            GetUserStatistic();
        }
        //await Navigation.PopModalAsync();
    }
}