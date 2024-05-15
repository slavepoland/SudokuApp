using Android.Service.Autofill;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuApp.Services
{
    public class SaveUserStatistics
    {
        private readonly string userstatistics = "statistics.txt";
        private readonly string UserStatisticsPath;
        private static readonly char[] separator = ['\n', '\r'];

        public SaveUserStatistics()
        {
            UserStatisticsPath = Path.Combine(Environment.GetFolderPath
                (Environment.SpecialFolder.LocalApplicationData), userstatistics);
        }

        public void DeleteUserStatisticsFile()
        {
            if (ExistUserStatisticsFile())
            {
                File.Delete(UserStatisticsPath);
            }
        }

        public void DeleteUserStatistics(string level)
        {
            while (IsFileInUse(UserStatisticsPath))
            {
                Thread.Sleep(50);
            }
            string[] listUserStatistics = ReadAndSplitFile();

            for (int i = 0; i < listUserStatistics.Length; i++)
            {
                string[] line = listUserStatistics[i].Split(';', StringSplitOptions.RemoveEmptyEntries);

                if (line[0] == level)
                {
                    // Modyfikuj odpowiednią linię na podstawie danych użytkownika
                    listUserStatistics[i] = $"{level};0;0;0;0;00:00;00:00;00:00;0;0\n\r";
                }
                else
                {
                    listUserStatistics[i] += $"\n\r";
                }
            }
            // Zapisz zmodyfikowane dane z powrotem do pliku
            File.WriteAllLines(UserStatisticsPath, listUserStatistics, Encoding.UTF8);
        }

        public bool ExistUserStatisticsFile()
        {
            try
            {
                if (File.Exists(UserStatisticsPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public void CreateUserStatisticsFile()
        {
            //Level[0]:GameStarted[1]:GameWon[2]:WinRate[3]:WinsWithoutMistakes[4]:BestTtime[5]:TotalTime[6]
            //AverageTime[7]:CurrentWinStreak[8]:BestWinStreak[9]
            try
            {
                File.Create(UserStatisticsPath, 1000000000 , FileOptions.Asynchronous);
                File.SetAttributes(UserStatisticsPath, FileAttributes.Normal);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            // Czekaj, aż plik zostanie zwolniony przez inny proces
            while (IsFileInUse(UserStatisticsPath))
            {
                Thread.Sleep(100); // Poczekaj 100 milisekund
            }

            List<string> listLevel = ["Easy", "Medium", "Hard", "Expert"];

            foreach(string level in listLevel)
            {
                try
                {
                    File.AppendAllText(UserStatisticsPath, 
                         $"{level};0;0;0;0;00:00;00:00;00:00;0;0\n\r", Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public void SaveUserStatisticsFile(UserStatistics userStatistics)
        {
            //Level[0]:GameStarted[1]:GameWon[2]:WinRate[3]:WinsWithoutMistakes[4]:BestTtime[5]:TotalTime[6]
            //AverageTime[7]:CurrentWinStreak[8]:BestWinStreak[9]
            // Czekaj, aż plik zostanie zwolniony przez inny proces
            while (IsFileInUse(UserStatisticsPath))
            {
                Thread.Sleep(50); // Poczekaj 100 milisekund
            }

            string[] listUserStatistics = ReadAndSplitFile();

            for (int i = 0; i < listUserStatistics.Length; i++)
            {
                string[] line = listUserStatistics[i].Split(';', StringSplitOptions.RemoveEmptyEntries);
                
                if (line[0] == userStatistics.Level)
                {
                    // Jeśli poziom odpowiada poziomowi użytkownika, zmodyfikuj linię
                    string bestTime; float winRate;
                    string totalTime;
                    string avgTime;
                    if (userStatistics.BestTime != 0)
                    {
                        int besttime = (int)TimeSpan.Parse(line[5]).TotalSeconds;
                        if (besttime > userStatistics.BestTime)
                        {
                            bestTime = TimeSpan.FromSeconds(userStatistics.BestTime).ToString(@"mm\:ss");
                        }
                        else
                        {
                            bestTime = besttime == 0 ? TimeSpan.FromSeconds(userStatistics.BestTime).ToString(@"mm\:ss") //@"hh\:mm\:ss"
                                : line[5];
                        }
                        totalTime = (TimeSpan.Parse(line[6]) + TimeSpan.FromSeconds(userStatistics.BestTime)).ToString();
                        avgTime = TimeSpan.FromSeconds((int)TimeSpan.Parse(totalTime).TotalSeconds / (int.Parse(line[2]) +  userStatistics.GameWon)).ToString(@"mm\:ss");
                    }
                    else
                    {
                        bestTime = line[5];
                        totalTime = line[6];
                        avgTime = line[7];
                    }
                    float testWon = float.Parse(line[2]) + float.Parse(userStatistics.GameWon.ToString());
                    float testStarted = float.Parse(line[1]) + float.Parse(userStatistics.GameStarted.ToString());
                    winRate = testWon / testStarted;  

                    // Modyfikuj odpowiednią linię na podstawie danych użytkownika
                    listUserStatistics[i] = $"{ userStatistics.Level};{int.Parse(line[1]) + userStatistics.GameStarted};" +
                                            $"{int.Parse(line[2]) + userStatistics.GameWon};{winRate:0.00%};" +
                                            $"{int.Parse(line[4]) + userStatistics.WinsWithoutMistakes};" +
                                            $"{bestTime};{totalTime};" +
                                            $"{avgTime};{userStatistics.CurrentWinStreak};" +
                                            $"{userStatistics.BestWinStreak}";
                    break; // Przerwij pętlę po znalezieniu i zmodyfikowaniu właściwej linii
                }
            }
            // Zapisz zmodyfikowane dane z powrotem do pliku
            File.WriteAllLines(UserStatisticsPath, listUserStatistics, Encoding.UTF8);
        }

        public string[] GetUserStatistics(string level)
        {
            //Level[0]:GameStarted[1]:GameWon[2]:WinRate[3]:WinsWithoutMistakes[4]:BestTtime[5]:TotalTime[6]
            //AverageTime[7]:CurrentWinStreak[8]:BestWinStreak[9]
            // Czekaj, aż plik zostanie zwolniony przez inny proces
            while (IsFileInUse(UserStatisticsPath))
            {
                Thread.Sleep(50); // Poczekaj 50 milisekund
            }

            string[] listUserStatistics = ReadAndSplitFile();

            for (int i = 0; i < listUserStatistics.Length; i++)
            {
                string[] line = listUserStatistics[i].Split(';', StringSplitOptions.RemoveEmptyEntries);

                // Jeśli poziom odpowiada poziomowi użytkownika, zwróć całą linię
                if (line[0] == level)
                {
                    return line;
                }
            }
            return [];
        }

        // Funkcja sprawdzająca, czy plik jest używany przez inny proces
        private static bool IsFileInUse(string path)
        {
            try
            {
                using FileStream fs = File.Open(path, FileMode.Open, 
                      FileAccess.Read, FileShare.None);
                fs.Close();
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        private string[] ReadAndSplitFile()
        {
            try
            {
                // Odczyt z pliku
                string fileContent = File.ReadAllText(UserStatisticsPath, Encoding.UTF8);

                // Podział na string[] na podstawie znaków nowej linii
                string[] lines = fileContent.Split(separator, 
                         StringSplitOptions.RemoveEmptyEntries);
                return lines;
            }
            catch (Exception)
            {
                return []; // Zwrócenie pustej tablicy w przypadku błędu
            }
        }
    }
}
