namespace Minesweeper
{
	public  class LevelSettings
	{
		public int RowCount { get; set; }

		public int ColumnCount { get; set; }

		public int MinesCount { get; set; }

		public int CellWidth { get; set; } = 50;

		public int CellHeight { get; set; } = 50;

		public LevelSettings(int rowCount, int columnCount, int minesCount)
		{
			RowCount = rowCount;
			ColumnCount = columnCount;
			MinesCount = minesCount;
		}
	}
}
