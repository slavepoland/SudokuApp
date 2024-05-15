using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuApp.Services
{
    public class GridCoordinates
    {
        public int RowSudokuBoardXaml { get; set; } //plansza z xaml z liniami oddzialjącymi 3x3
        public int ColumnSudokuBoardXaml { get; set; } //plansza z xaml z liniami oddzialjącymi 3x3
        public int RowBoardSudoku { get; set; } //oryginalne koordynaty planszy sudoku 9x9
        public int ColumnBoardSudoku { get; set; } //oryginalne koordynaty planszy sudoku 9x9
    }
}
