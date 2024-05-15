namespace SudokuApp.Services
{
    public class UserStatistics()
    {
        public string Level { get; set; }
        public int GameStarted { get; set; }
        public int GameWon { get; set; }
        public float WinRate { get; set; }
        public int WinsWithoutMistakes { get; set; }
        public int BestTime { get; set; }
        public int TotalTime { get; set; }
        public int AverageTime { get; set; }
        public int CurrentWinStreak { get; set; }
        public int BestWinStreak { get; set; }
    }
}
