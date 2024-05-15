using Microsoft.Maui.Storage;
using System.IO;
using System.Text.Json;

namespace SudokuApp.Services
{
    public class SaveCurrentGame
    {
        public int[,] BoardUser { get; set; }
        //public int[,] BoardOryginal { get; set; }

        public SaveCurrentGame()
        {
            BoardUser = new int [9, 9];
        }

        public void SaveSudokuBoard(int[,] boarduser, string fileName)
        {
            // Utwórz pełną ścieżkę dostępu do pliku
            string filePath = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), fileName);

            string jsonString = JsonSerializer.Serialize(ConvertArrayToList(boarduser));
            File.WriteAllText(filePath, jsonString);
        }

        public int[,] LoadSudokuBoard(string fileName)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), fileName);

            string jsonString = File.ReadAllText(filePath);
            List<List<int>> list = ParseStringToList(jsonString);
            return ConvertListToArray(list);
        }

        public List<List<int>> ParseStringToList(string data)
        {
            List<List<int>> resultList = [];

            // Usuń nawiasy kwadratowe z początku i końca stringa
            data = data.Trim('[', ']');

            // Rozdziel string na listę ciągów znaków za pomocą przecinków
            string[] listStrings = data.Split("],[");

            // Sprawdź, czy liczba wierszy jest równa 9
            if (listStrings.Length != 9)
            {
                throw new ArgumentException("Lista list musi zawierać dokładnie 9 wierszy.");
            }

            foreach (string listString in listStrings)
            {
                List<int> intList = [];

                // Usuń nawiasy kwadratowe z początku i końca ciągu znaków
                string[] numbers = listString.Trim('[', ']').Split(',');

                foreach (string number in numbers)
                {
                    // Konwertuj ciąg znaków na liczbę i dodaj do listy intList
                    if (int.TryParse(number, out int value))
                    {
                        intList.Add(value);
                    }
                    else
                    {
                        // W razie niepowodzenia konwersji można obsłużyć błąd lub zignorować liczbę
                        Console.WriteLine($"Failed to parse number: {number}");
                    }
                }
                // Dodaj intList do listy wynikowej
                resultList.Add(intList);
            }
            return resultList;
        }

        public static int[,] ConvertListToArray(List<List<int>> list)
        {
            int rows = list.Count;
            int columns = list[0].Count;

            int[,] array = new int[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    array[i, j] = list[i][j];
                }
            }
            return array;
        }

        public List<List<int>> ConvertArrayToList(int[,] array)
        {
            List<List<int>> list = [];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                List<int> sublist = [];
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    sublist.Add(array[i, j]);
                }
                list.Add(sublist);
            }
            return list;
        }
    }
}
