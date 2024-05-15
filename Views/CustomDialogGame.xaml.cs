using Mopups.Services;

namespace SudokuApp.Views;

public partial class CustomDialogGame : Mopups.Pages.PopupPage
{
    public bool Result { get; private set; }

    private readonly TaskCompletionSource<bool> _tcs = new();

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
    public static async Task CloseDialogAsync()
    {
        // Usuniêcie okna dialogowego ze stosu
        while (MopupService.Instance.PopupStack.Count > 0)
        {
            await MopupService.Instance.PopAsync();
        };
    }

    // Dodaj w³aœciwoœæ ResultTask, która zwraca zadanie, które zostanie ukoñczone, gdy okno dialogowe zostanie zamkniête
    public Task<bool> ResultTask => _tcs.Task;

    public Task<bool> WaitForDialogResultAsync()
    {
        return _tcs.Task;
    }

    public CustomDialogGame(string title, string message, string yesButtonText, string noButtonText)
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

        lblTitle.Text = title;

        lblMessage.Text = message;

        lblYes.Text = yesButtonText;

        lblNo.Text = noButtonText;
    }
}