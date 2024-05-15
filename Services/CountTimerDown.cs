using System.Timers;

namespace SudokuApp.Services
{
    public class CountTimerDown
    {
        private int countdownSeconds;
        private readonly System.Timers.Timer timer;
        public bool IsPaused { get; set; }


        public event EventHandler<int> CountdownTick;
        public event EventHandler CountdownFinished;


        public CountTimerDown(int seconds)
        {
            countdownSeconds = seconds;
            timer = new System.Timers.Timer(1000); // Timer ticks every 1 second (1000 milliseconds)
            timer.Elapsed += TimerElapsedDown; //cout down
        }

        public void Start()
        {      
            if (!IsPaused)
            {
                timer.Start();
            }
            else
            {
                Resume();
            }
        }

        public void Pause()
        {
            IsPaused = true;
            timer.Stop();
        }

        public void Resume()
        {
            IsPaused = false;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            IsPaused = false;
            countdownSeconds = 0;
        }

        private void TimerElapsedDown(object sender, ElapsedEventArgs e)
        {
            _ = MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (countdownSeconds > 0)
                {
                    countdownSeconds--;

                    CountdownTick.Invoke(this, countdownSeconds);
                }
                else
                {
                    Stop();
                    CountdownFinished.Invoke(this, EventArgs.Empty);
                }
            });
        }
    }
}
