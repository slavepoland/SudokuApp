using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuApp.Services
{
    public class PlayerMove
    {
        public List<Move> moveHistory = [];

        /// <summary>
        /// Zapisz ostatni ruch gracza
        /// </summary>
        /// <param name="rowxaml"></param>
        /// <param name="columnxaml"></param>
        /// <param name="rowsudoku"></param>
        /// <param name="columnsudoku"></param>
        /// <param name="previousValue"></param>
        /// <param name="newValue"></param>
        public void SaveMove(int rowxaml, int columnxaml, int rowsudoku, int columnsudoku, int previousValue, int newValue)
        {
            var move = new Move
            {
                RowSudokuBoardXaml = rowxaml,
                ColumnSudokuBoardXaml = columnxaml,
                RowBoardSudoku = rowsudoku,
                ColumnBoardSudoku = columnsudoku,
                PreviousValue = previousValue,
                NewValue = newValue
            };

            moveHistory.Add(move);

            if (moveHistory.Count > 3)
            {
                moveHistory.RemoveAt(0);
            }
        }

        /// <summary>
        /// Cofnij ostatni ruch gracza
        /// </summary>
        /// <returns>Move</returns>
        public Move UndoLastMoves()
        {
            if(moveHistory.Count >= 1)
            {
                Move lastMove = moveHistory.Last();
                //usuń cofany ruch
                moveHistory.Remove(lastMove);
                return lastMove;
            }
            return null;
        }
    }

    public class Move
    {
        public int RowSudokuBoardXaml { get; set; } //plansza z xaml z liniami oddzialjącymi 3x3
        public int ColumnSudokuBoardXaml { get; set; } //plansza z xaml z liniami oddzialjącymi 3x3
        public int RowBoardSudoku { get; set; } //oryginalne koordynaty planszy sudoku 9x9
        public int ColumnBoardSudoku { get; set; } //oryginalne koordynaty planszy sudoku 9x9
        public int PreviousValue { get; set; }
        public int NewValue { get; set; }
    }
}
