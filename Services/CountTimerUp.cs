using System.Timers;

namespace SudokuApp.Services
{
    public class CountTimerUp
    {
        private int countUpSeconds;
        private readonly System.Timers.Timer timer;
        public bool IsPaused { get; set; }

        public event EventHandler<int> CountdownTick;
        public event EventHandler CountdownFinished;


        public CountTimerUp(int seconds)
        {
            countUpSeconds = seconds;
            timer = new System.Timers.Timer(1000); // Timer ticks every 1 second (1000 milliseconds)
            timer.Elapsed += TimerElapsedUp; //count up
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
            countUpSeconds = -1;
        }

        private void TimerElapsedUp(object sender, ElapsedEventArgs e)
        {
            _ = MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (countUpSeconds >= 0)
                {
                    countUpSeconds++;

                    CountdownTick.Invoke(this, countUpSeconds);
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

