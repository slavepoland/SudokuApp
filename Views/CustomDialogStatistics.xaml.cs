using Mopups.Services;

namespace SudokuApp.Views;

public partial class CustomDialogStatistics : Mopups.Pages.PopupPage
{
    public bool Result { get; private set; }

    private readonly TaskCompletionSource<bool> _tcs = new();

    // Dodaj w³aœciwoœæ ResultTask, która zwraca zadanie, które zostanie ukoñczone, gdy okno dialogowe zostanie zamkniête
    public Task<bool> ResultTask => _tcs.Task;

    public async Task Frame_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame)
        {
            while (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            };

            switch (frame.ClassId)
            {
                case "Yes":
                    _tcs.TrySetResult(true);
                    break;
                case "No":
                    _tcs.TrySetResult(false);
                    break;
            }
        }
    }

    // Metoda zamykaj¹ca okno dialogowe
    private static async Task CloseDialogAsync()
    {
        // Usuniêcie okna dialogowego ze stosu
        while (MopupService.Instance.PopupStack.Count > 0)
        {
            await MopupService.Instance.PopAsync();
        };
    }

    public CustomDialogStatistics(string message, string yesButtonText, string noButtonText)
    {
        InitializeComponent();

        frameYes.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                _tcs.TrySetResult(true); // Ustawienie wyniku na true
                await CloseDialogAsync();
            })
        });

        frameNo.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                _tcs.TrySetResult(false); // Ustawienie wyniku na false
                await CloseDialogAsync();
            })
        });

        lblMessage.Text = message;

        lblYes.Text = yesButtonText;

        lblNo.Text = noButtonText;

        //var titleLabel = new Label
        //{
        //    Text = title,
        //    FontAttributes = FontAttributes.Bold,
        //    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
        //    HorizontalOptions = LayoutOptions.Center
        //};

        //var messageLabel = new Label
        //{
        //    FontAttributes = FontAttributes.Bold,
        //    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
        //    Text = message,
        //    TextColor = textColor,
        //    HorizontalOptions = LayoutOptions.Center
        //};

        //var yesButton = new Button
        //{
        //    Text = yesButtonText,
        //    TextColor = textColor, // Ustawienie koloru tekstu przycisku "Tak"
        //    CornerRadius = 10, // Zaokr¹glenie rogów przycisku
        //    HorizontalOptions = LayoutOptions.CenterAndExpand
        //};

        //yesButton.Clicked += async (sender, args) =>
        //{
        //    Result = true; // Ustawienie wartoœci na true, gdy klikniêto przycisk "Tak"
        //    //_ = await Navigation.PopAsync();
        //    while (MopupService.Instance.PopupStack.Count > 0)
        //    {
        //        await MopupService.Instance.PopAsync();
        //    };
        //};

        //var noButton = new Button
        //{
        //    Text = noButtonText,
        //    TextColor = textColor, // Ustawienie koloru tekstu przycisku "Nie"
        //    CornerRadius = 10, // Zaokr¹glenie rogów przycisku
        //    HorizontalOptions = LayoutOptions.CenterAndExpand
        //};

        //noButton.Clicked += async (sender, args) =>
        //{
        //    Result = false; // Ustawienie wartoœci na false, gdy klikniêto przycisk "Nie"
        //    //_ = await Navigation.PopAsync();
        //    while (MopupService.Instance.PopupStack.Count > 0)
        //    {
        //        await MopupService.Instance.PopAsync();
        //    };
        //};

        //var layout = new StackLayout
        //{
        //    Padding = new Thickness(20),
        //    Children = { titleLabel, messageLabel, yesButton, noButton },
        //    HorizontalOptions = LayoutOptions.FillAndExpand,
        //    VerticalOptions = LayoutOptions.FillAndExpand,
        //};

        //var frame = new Frame
        //{
        //    CornerRadius = 10,
        //    HorizontalOptions = LayoutOptions.FillAndExpand,
        //    VerticalOptions = LayoutOptions.FillAndExpand,
        //    Content = layout,
        //};

        //Content = frame;

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