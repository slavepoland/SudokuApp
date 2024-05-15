using Mopups.Services;

namespace SudokuApp.Views;

public partial class CustomDialogGameFinalText : Mopups.Pages.PopupPage
{
    // Metoda zamykaj¹ca okno dialogowe
    private static async Task CloseDialogAsync()
    {
        // Usuniêcie okna dialogowego ze stosu
        while (MopupService.Instance.PopupStack.Count > 0)
        {
            await MopupService.Instance.PopAsync();
        };
    }

    public CustomDialogGameFinalText(string title, string message, string yesButtonText)
    {
        InitializeComponent();

        frameOK.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                await CloseDialogAsync();
            })
        });

        lblTitle.Text = title;

        lblMessage.Text = message;

        lblYes.Text = yesButtonText;

    }

}