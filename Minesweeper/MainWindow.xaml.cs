using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Minesweeper
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private LevelSettings settings = new LevelSettings(5, 5, 10);
		private int rowCount = 0;
		private int columnCount = 0;

		private Cell[,] gameField;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Btn_Click(object sender, RoutedEventArgs e)
		{
			//получение значения лежащего в Tag
			int n = (int)((Button)sender).Tag;
			//установка фона нажатой кнопки, цвета и размера шрифта
			((Button)sender).Background = Brushes.White;
			((Button)sender).Foreground = Brushes.Red;
			((Button)sender).FontSize = 23;
			//запись в нажатую кнопку её номера
			((Button)sender).Content = n.ToString();
		}

		private void Btn_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				MessageBox.Show("Нажата левая кнопка мыши!");
			}
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				MessageBox.Show("Нажата средняя кнопка мыши!");
			}
			if (e.RightButton == MouseButtonState.Pressed)
			{
				MessageBox.Show("Нажата правая кнопка мыши!");
			}
		}

		#region Menu events

		private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void RulesMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var rules = @"На поле в некоторых ячейках спрятаны мины. Общее количество мин указано вверху в информационной панели. Цель игры -найти все мины на поле и пометить такие ячейки флажком.

Игра начинается с первого клика по любой ячейке на поле. При клике на ячейке она открывается. Если в ней находится мина, то на поле открываются все мины и игра заканчивается проигрышем. Если в самой ячейке мины нет, но есть мины в соседних ячеках, то отображается число, соответствующее количеству мин в соседних ячейках. Если ни в самой ячейке, ни в соседних мин нет, то ячейка остается пустой и открываются все соседние пустые яйчеки до тех пор, пока не будут достигнуты ячейки с ненулевой информацией о количестве мин.

С помощью клика с нажатым Shift можно пометить ячейку флажком(т.е.как содержащую мину). Перед началом игры можно указать количество мин на поле. Чем больше мин спрятано, тем труднее играть. 

По умолчанию на поле размещается 10 мин. После того, как на игровом поле останутся неоткрытыми только ячейки, предположительно содержащие мины, то игра проверит результат и выведет сообщение о успешности или ошибочности предложенного варианта размещения мин.";
			MessageBox.Show(rules, "Правила игры", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(
				"Контрольная работа \nРазработка игрового приложения",
				"О программе",
				MessageBoxButton.OK,
				MessageBoxImage.Information);
		}

		#endregion

		private void LevelSelected_Click(object sender, RoutedEventArgs e)
		{
			var button = (RadioButton)sender;
			switch (button.Name)
			{
				case "ProfiLevel":
					settings = new LevelSettings(20, 20, 20)
					{
						CellHeight = 30,
						CellWidth = 30
					};
					break;

				case "BeginnerLevel":
					settings = new LevelSettings(5, 5, 5);
					break;

				case "MiddleLevel":
					settings = new LevelSettings(10, 10, 10);
					break;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			StartNewGame();
		}

		private void StartNewGame()
		{
			gameField = new Cell[settings.RowCount, settings.ColumnCount];
			for (int i = 0; i < settings.RowCount; i++)
				for (int j = 0; j < settings.ColumnCount; j++)
					gameField[i, j] = new Cell();

			PlantMines(settings.MinesCount);

			ugr.Children.Clear();
			rowCount = settings.RowCount;
			columnCount = settings.ColumnCount;

			ugr.Width = columnCount * settings.CellWidth;
			ugr.Height = rowCount * settings.CellHeight;
			Width = ugr.Width + 20;
			Height = ugr.Height + 60;

			for (int i = 0; i < rowCount * columnCount; i++)
			{
				//создание кнопки
				Button btn = new Button
				{
					//запись номера кнопки
					Tag = i,
					//установка размеров кнопки
					Width = settings.CellWidth,
					Height = settings.CellHeight,
					//текст на кнопке
					Content = " ",
					//толщина границ кнопки
					Margin = new Thickness(2)
				};
				//при нажатии кнопки, будет вызываться метод Btn_Click
				btn.Click += Btn_Click;
				//btn.PreviewMouseDown += Btn_MouseDown;
				//добавление кнопки в сетку
				ugr.Children.Add(btn);
			}
		}

		private void NewGame_Click(object sender, RoutedEventArgs e)
		{
			StartNewGame();
		}

		// Алгоритм работы функции будет выглядеть следующим образом:
		// 1.	Выбрать в пределах поля случайную ячейку.
		// 2.	Проверить, что в этой ячейке ещё нет мины (если есть, вернуться к шагу 1).
		// 3.	Проверить, что рядом с этой ячейкой есть хотя бы одна пустая(если нет, вернуться к шагу 1).
		// 4.	Разместить в выбранной ячейке мину(записать в массив 9).
		private void PlantMines(int count)
		{
			// Генератор случайных чисел
			var random = new Random();
			var availableColumnIndexes = Enumerable.Range(1, settings.ColumnCount).ToList();
			var availableRowIndexes = Enumerable.Range(1, settings.RowCount).ToList();

			// Условие завершения цикла
			// 1. Все мины расставлены (count = 0)
			// 2. Все ячейки пройдены (availableColumnIndexes и availableRowIndexes не содержат элементов)
			while (count != 0 || (!availableRowIndexes.Any() && !availableColumnIndexes.Any()))
			{
				// Случайный индекс строки
				var rowIndex = random.Next(availableRowIndexes.Count);
				availableRowIndexes.Remove(rowIndex);

				// Случайный индекс колонки
				var columnIndex = random.Next(availableColumnIndexes.Count);
				availableColumnIndexes.Remove(columnIndex);

				if (CanMining(rowIndex, columnIndex, settings.RowCount, settings.ColumnCount))
				{
					gameField[rowIndex, columnIndex].Value = 9;
					count--;
				}
			}
		}

		// Координаты соседних ячеек (i-1, j-1), (i, j-1), (i+1, j-1), (i-1, j), (i+1, j), (i-1, j+1), (i, j+1), (i-1, j+1).
		private bool CanMining(int i, int j, int rowCount, int columnCount)
		{
			if (gameField[i, j].Value == 9)
				return false;

			var related = new Tuple<int, int>[]
			{
				new Tuple<int, int>(i - 1, j - 1),
				new Tuple<int, int>(i, j - 1),
				new Tuple<int, int>(i + 1, j - 1),
				new Tuple<int, int>(i - 1, j),
				new Tuple<int, int>(i + 1,j),
				new Tuple<int, int>(i - 1,j + 1),
				new Tuple<int, int>(i ,j + 1),
				new Tuple<int, int>(i - 1,j + 1)
			};

			return CheckRelatedValues(
				related,
				(x, y) => x >= 1 && x <= settings.RowCount - 1 && y >= 1 && y <= settings.ColumnCount - 1);
		}

		private bool CheckRelatedValues(Tuple<int, int>[] related, Func<int, int, bool> condition)
		{
			foreach (var item in related)
			{
				if (condition(item.Item1, item.Item2))
				{
					var value = gameField[item.Item2, item.Item2].Value;
					if (value != 9)
						return true;
				}
			}
			return false;
		}

		private void SetValue()
		{ 
		
		}
	}
}
