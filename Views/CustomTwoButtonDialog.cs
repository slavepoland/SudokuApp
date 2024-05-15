using Android.Content.Res;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using System.Threading.Tasks;


namespace SudokuApp.Views;

public class CustomTwoButtonDialog : ContentPage
{
    // W³aœciwoœæ Result, która przechowuje wartoœæ zwracan¹ przez dialog
    public bool Result { get; private set; }
    private readonly Color Primary = Color.FromArgb("#512BD4");
    private readonly Color MyPrimary = Color.FromArgb("#2196F3");
    private Color textColor;

    public CustomTwoButtonDialog(string title, string message, string yesButtonText, string noButtonText)
    {
        textColor = Application.Current.RequestedTheme == AppTheme.Light ? MyPrimary : Primary;

        var titleLabel = new Label
        {
            Text = title,
            FontAttributes = FontAttributes.Bold,
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            HorizontalOptions = LayoutOptions.Center
        };

        var messageLabel = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            Text = message,
            TextColor = textColor,
            HorizontalOptions = LayoutOptions.Center
        };

        var yesButton = new Button
        {
            Text = yesButtonText,
            TextColor = textColor, // Ustawienie koloru tekstu przycisku "Tak"
            CornerRadius = 10, // Zaokr¹glenie rogów przycisku
            HorizontalOptions = LayoutOptions.CenterAndExpand
        };

        yesButton.Clicked += async (sender, args) =>
        {
            Result = true; // Ustawienie wartoœci na true, gdy klikniêto przycisk "Tak"
            _ = await Navigation.PopAsync();
        };

        var noButton = new Button
        {
            Text = noButtonText,
            TextColor = textColor, // Ustawienie koloru tekstu przycisku "Nie"
            CornerRadius = 10, // Zaokr¹glenie rogów przycisku
            HorizontalOptions = LayoutOptions.CenterAndExpand
        };

        noButton.Clicked += async (sender, args) =>
        {
            Result = false; // Ustawienie wartoœci na false, gdy klikniêto przycisk "Nie"
            _ = await Navigation.PopAsync();
        };

        var layout = new StackLayout
        {
            Padding = new Thickness(20),
            Children = { titleLabel, messageLabel, yesButton, noButton },
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,
        };

        var frame = new Frame
        {
            CornerRadius = 10, 
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,
            Content = layout,
        };

        Content = frame;

        //var absoluteLayout = new AbsoluteLayout
        //{
        //    BackgroundColor = Color.FromArgb("#00FFFFFF"),
        //};

        //// Ustawienie po³o¿enia i zachowania ramki w AbsoluteLayout
        //AbsoluteLayout.SetLayoutBounds(frame, new Rect(0.5, 0.5, 0.5, 0.2)); // Po³o¿enie i rozmiar ramki
        //AbsoluteLayout.SetLayoutFlags(frame, AbsoluteLayoutFlags.All); // Ustawienie flag dla zachowania ramki

        //absoluteLayout.Children.Add(frame);

        //Content = absoluteLayout;
    }
}