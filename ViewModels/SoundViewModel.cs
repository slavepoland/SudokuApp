using Android.Media;

namespace SudokuApp.ViewModels
{
    public class SoundViewModel : BaseViewModel
    {
        private bool _isSoundEnabled;
        private MediaPlayer _mediaPlayer;

        public SoundViewModel()
        {
            try
            {
                _isSoundEnabled = true;
                var soundFilePath = GetSoundFilePath("loud-thud.mp3"); // Zmień na odpowiednią nazwę pliku dźwiękowego
                _mediaPlayer = new MediaPlayer();
                _mediaPlayer.SetDataSource(soundFilePath);
                _mediaPlayer.Prepare();
            }
            catch (Exception) { }
        }

        public bool IsSoundEnabled
        {
            get => _isSoundEnabled;
            set
            {
                if (_isSoundEnabled != value)
                {
                    _isSoundEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public void PlaySound()
        {
            if (_isSoundEnabled)
            {
                _mediaPlayer.Start();
            }
        }

        public void Dispose()
        {
            // Zatrzymaj odtwarzanie i zwolnij zasoby
            _mediaPlayer?.Stop();
            _mediaPlayer?.Release();
            _mediaPlayer = null;
        }

        private string GetSoundFilePath(string fileName)
        {
            using (var assetStream = Android.App.Application.Context.Assets.Open("Audio/" + fileName)) // 
            {
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    assetStream.CopyTo(fileStream);
                }
                return filePath;
            }
        }
    }
}
