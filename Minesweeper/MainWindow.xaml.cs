using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int flagsCount = 0;

		private LevelSettings settings = new LevelSettings(5, 5, 10);

		private Cell[,] gameField;

		private Func<int, int, Tuple<int, int>[]> relatedCells =
			(i, j) => new Tuple<int, int>[]
			{
				new Tuple<int, int>(i - 1, j - 1),
				new Tuple<int, int>(i, j - 1),
				new Tuple<int, int>(i + 1, j - 1),
				new Tuple<int, int>(i - 1, j),
				new Tuple<int, int>(i + 1,j),
				new Tuple<int, int>(i - 1,j + 1),
				new Tuple<int, int>(i, j + 1),
				new Tuple<int, int>(i - 1,j + 1)
			};

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Btn_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var button = sender as Button;

			// Открыть клетку
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				OpenCell(button);
			}

			// Показать соседние клетки
			if (e.MiddleButton == MouseButtonState.Pressed)
			{

			}

			// Пометить флажком
			if (e.RightButton == MouseButtonState.Pressed)
			{
				BitmapImage flag = new BitmapImage(new Uri(@"pack://application:,,,/Images/Флаг.png", UriKind.Absolute));
				button.Background = Brushes.Yellow;
				SetImage(button, flag);
			}
		}

		// Вернуть исходное состояние соседних клеток
		private void Btn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Released)
			{
			}
		}

		private void OpenCell(Button button)
		{
			if (button == null)
				return;


			//установка фона нажатой кнопки, цвета и размера шрифта
			button.Background = Brushes.White;

			button.FontSize = settings.CellWidth / 2;
			//запись в нажатую кнопку её номера
			DrawCellContent(button);
		}

		private void DrawCellContent(Button button)
		{
			//получение значения лежащего в Tag
			var cell = (Cell)button.Tag;

			if (cell.Value > 0 && cell.Value < 9)
				button.Content = cell.Value;

			switch (cell.Value)
			{
				case 0:
					button.Content = "";
					break;

				case 1:
					button.Foreground = Brushes.Red;
					break;

				case 2:
					button.Foreground = Brushes.Green;
					break;

				case 3:
					button.Foreground = Brushes.Blue;
					break;

				case 4:
					button.Foreground = Brushes.Aqua;
					break;

				case 5:
					button.Foreground = Brushes.DarkBlue;
					break;

				case 6:
					button.Foreground = Brushes.DarkCyan;
					break;

				case 7:
					button.Foreground = Brushes.Yellow;
					break;

				case 8:
					button.Foreground = Brushes.DarkRed;
					break;

				case 9:
					//создание и инициализация глобальной переменной для хранения изображения мины
					BitmapImage mine = new BitmapImage(new Uri(@"pack://application:,,,/Images/Мина.png", UriKind.Absolute));
					button.Background = Brushes.Red;
					SetImage(button, mine);
					break;
			};
		}

		private void SetImage(Button button, BitmapImage image)
		{
			//создание контейнера для хранения изображения
			Image img = new Image
			{
				//запись картинки с миной в контейнер
				Source = image
			};

			//создание компонента для отображения изображения
			StackPanel stackPnl = new StackPanel
			{
				//установка толщины границ компонента
				Margin = new Thickness(1)
			};

			//добавление контейнера с картинкой в компонент
			stackPnl.Children.Add(img);
			//запись компонента в кнопку
			button.Content = stackPnl;
		}

		#region Menu events

		private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void RulesMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var rules = @"На поле в некоторых ячейках спрятаны мины. Цель игры - найти все мины на поле и пометить такие ячейки флажком.

Игра начинается с первого клика по любой ячейке на поле. При клике на ячейке она открывается. Если в ней находится мина, то на поле открываются все мины и игра заканчивается проигрышем. Если в самой ячейке мины нет, но есть мины в соседних ячеках, то отображается число, соответствующее количеству мин в соседних ячейках. Если ни в самой ячейке, ни в соседних мин нет, то ячейка остается пустой и открываются все соседние пустые яйчеки до тех пор, пока не будут достигнуты ячейки с ненулевой информацией о количестве мин.

С помощью клика правой кнопкой мыши можно пометить ячейку флажком(т.е.как содержащую мину). 

После того, как на игровом поле останутся неоткрытыми только ячейки, предположительно содержащие мины, то игра проверит результат и выведет сообщение о успешности или ошибочности предложенного варианта размещения мин.";
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
			// Инициализация массива начальными значениями
			InitArray();

			// Расстановка мин
			PlantMines(settings.MinesCount);

			// Подсчет числа мин в соседних ячейках
			SetValues();

			// Отрисовка поля
			DrawField();
		}

		private void DrawField()
		{
			// Очистка поля
			ugr.Children.Clear();

			// Размеры поля
			ugr.Width = settings.ColumnCount * settings.CellWidth;
			ugr.Height = settings.RowCount * settings.CellHeight;

			// Размеры главного окна
			Width = ugr.Width + 20;
			Height = ugr.Height + 60;

			for (int i = 0; i < settings.ColumnCount; i++)
				for (int j = 0; j < settings.RowCount; j++)
				{
					//создание кнопки
					Button btn = new Button
					{
						// В Tag записываем элемент массива
						Tag = gameField[i, j],
						//установка размеров кнопки
						Width = settings.CellWidth,
						Height = settings.CellHeight,
						//текст на кнопке
						Content = " ",
						//толщина границ кнопки
						Margin = new Thickness(2)
					};
					btn.PreviewMouseDown += Btn_MouseDown;
					btn.PreviewMouseUp += Btn_MouseUp;
					//добавление кнопки в сетку
					ugr.Children.Add(btn);
				}
		}

		private void InitArray()
		{
			gameField = new Cell[settings.RowCount, settings.ColumnCount];
			for (int i = 0; i < settings.ColumnCount; i++)
				for (int j = 0; j < settings.RowCount; j++)
					gameField[i, j] = new Cell();
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

				if (CanMining(rowIndex, columnIndex))
				{
					gameField[rowIndex, columnIndex].Value = 9;
					count--;
				}
			}
		}

		// Координаты соседних ячеек (i-1, j-1), (i, j-1), (i+1, j-1), (i-1, j), (i+1, j), (i-1, j+1), (i, j+1), (i-1, j+1).
		private bool CanMining(int i, int j)
		{
			// Мина уже установлена
			if (gameField[i, j].Value == 9)
				return false;

			// Проверка смежных ячеек
			foreach (var item in relatedCells(i, j))
			{
				if (IsItemExists(item.Item1, item.Item2))
				{
					// Хотя бы одна ячейка свободна
					var value = gameField[item.Item1, item.Item2].Value;
					if (value != 9)
						return true;
				}
			}

			return false;
		}

		// Установка значений массива
		private void SetValues()
		{
			for (int i = 0; i < settings.ColumnCount; i++)
				for (int j = 0; j < settings.ColumnCount; j++)
					gameField[i, j].Value = CalculateValue(i, j);
		}

		// Подсчет мин в смежных ячейках
		private int CalculateValue(int i, int j)
		{
			int count = 0;

			if (gameField[i, j].Value == 9)
				return 9;

			foreach (var item in relatedCells(i, j))
			{
				if (IsItemExists(item.Item1, item.Item2))
				{
					var value = gameField[item.Item1, item.Item2].Value;
					if (value == 9)
						count++;
				}
			}

			return count;
		}

		private bool IsItemExists(int i, int j)
		{
			return i >= 1 &&
				i <= settings.RowCount - 1 &&
				j >= 1 &&
				j <= settings.ColumnCount - 1;
		}
	}
}
