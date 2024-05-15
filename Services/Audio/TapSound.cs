using SudokuApp.ViewModels;
//using Android.Media;


namespace SudokuApp.Services.Audio
{
    static class TapSound
    {
        private static SoundViewModel _viewModel;

        public static void PlaySound()
        {
            if (Preferences.Default.Get("UserSound", "") == "True")
            {
                Task.Run(() =>
                {
                    if (Application.Current.MainPage?.BindingContext is SoundViewModel viewModel)
                    {
                        // Ustaw SoundViewModel jako BindingContext dla tej strony
                        _viewModel = viewModel;
                    }
                    _viewModel?.PlaySound();
                });
            }
        }
    }

    //public class PlaySoundAndroid
    //{
    //    private MediaPlayer _mediaPlayer;

    //    public void PlaySystemSound()
    //    {
    //        try
    //        {
    //            // Utwórz obiekt MediaPlayer i odtwórz dźwięk systemowy
    //            _mediaPlayer = MediaPlayer.Create(Android.App.Application.Context, 
    //                global::Android.Provider.Settings.System.DefaultNotificationUri); //DefaultNotificationUri
    //            _mediaPlayer.Start();
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex.ToString());
    //        }
    //    }

    //    public void StopSystemSound()
    //    {
    //        _mediaPlayer.Stop();
    //    }
    //}
}
