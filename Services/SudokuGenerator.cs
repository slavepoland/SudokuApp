using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuApp.Services
{
    public class SudokuGenerator
    {
        private int[,] BoardSolved { get; set; }
        private int[,] Board { get; set; }
        private readonly Random random = new();

        public SudokuGenerator(int level)
        {
            Board = new int[9, 9];
            BoardSolved = new int[9, 9];
            GenerateSudoku(level);
        }

        public int[,] GetBoard()
        {
            return (int[,])Board.Clone();
        }

        public int[,] GetBoardOryg()
        {
            return (int[,])BoardSolved.Clone();
        }

        private void GenerateSudoku(int level)
        {
            SudokuSolver solver = new();
            solver.Solve();

            // Skopiuj planszę rozwiązania
            Board = solver.GetBoard();
            BoardSolved = solver.GetBoard();
            RandomlyRemoveNumbers(level);
        }

        private void RandomlyRemoveNumbers(int level)
        {
            int removenumber = level switch
            {
                1 => SudokuGeneratorHelpers.easy, 
                2 => SudokuGeneratorHelpers.medium,
                3 => SudokuGeneratorHelpers.hard,
                4 => SudokuGeneratorHelpers.expert,
                _ => SudokuGeneratorHelpers.easy,
            };
            for (int i = 0; i < removenumber; i++)  // Dostosuj ilość usuwanych liczb
            {
                int row = random.Next(0, 9);
                int col = random.Next(0, 9);

                if (Board[row, col] != -1)
                {
                    Board[row, col] = -1;
                }
                else
                {
                    i--;  // Jeśli to pole było już puste, powtórz próbę
                }
            }
        }
    }
}
