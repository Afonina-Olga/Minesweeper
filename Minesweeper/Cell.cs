namespace Minesweeper
{
	// Состояние ячейки
	public class Cell
	{
		// Значение от 0 до 9
		public int Value { get; set; } = 0;

		// Ячейка открыта,
		public bool IsOpen { get; set; } = false;

		// Ячейка отмечена флажком?
		public bool IsMarked { get; set; } = false;

		public int RowIndex { get; set; }

		public int CellIndex { get; set; }
	}
}
