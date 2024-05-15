using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuApp.Services
{
    public class SudokuSolver
    {
        private readonly int[,] board;

        public SudokuSolver()
        {
            board = new int[9, 9];
            InitializeFirstRowRandomly();
        }

        private void InitializeFirstRowRandomly()
        {
            List<int> numbers = Enumerable.Range(1, 9).ToList();
            Random random = new();

            for (int col = 0; col < 9; col++)
            {
                int randomIndex = random.Next(random.Next(numbers.Count));
                board[0, col] = numbers[randomIndex];
                numbers.RemoveAt(randomIndex);
            }
        }

        public int[,] GetBoard()
        {
            return (int[,])board.Clone();
        }

        public bool Solve()
        {
            if (!FindUnassignedLocation(out int row, out int col))
            {
                // Brak wolnych miejsc, sudoku rozwiązane
                return true;
            }

            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(row, col, num))
                {
                    board[row, col] = num;

                    if (Solve())
                    {
                        return true;
                    }

                    // Jeśli wstawienie liczby nie prowadzi do rozwiązania, cofnij
                    board[row, col] = 0;
                }
            }

            // Brak możliwości wstawienia liczby, cofnij
            return false;
        }

        private bool FindUnassignedLocation(out int row, out int col)
        {
            for (row = 0; row < 9; row++)
            {
                for (col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0)
                    {
                        return true;
                    }
                }
            }

            row = -1;
            col = -1;
            return false;
        }

        private bool IsSafe(int row, int col, int num)
        {
            return !UsedInRow(row, num) && !UsedInCol(col, num) && !UsedInBox(row - row % 3, col - col % 3, num);
        }

        private bool UsedInRow(int row, int num)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] == num)
                {
                    return true;
                }
            }

            return false;
        }

        private bool UsedInCol(int col, int num)
        {
            for (int row = 0; row < 9; row++)
            {
                if (board[row, col] == num)
                {
                    return true;
                }
            }

            return false;
        }

        private bool UsedInBox(int boxStartRow, int boxStartCol, int num)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[boxStartRow + row, boxStartCol + col] == num)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
