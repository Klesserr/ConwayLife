using System;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.IO.IsolatedStorage;

namespace ConwayLife // Note: actual namespace depends on the project name.
{
	internal class Program
	{
		static void Main(string[] args)
		{
			int[,] cells =
			{
			{1,1,1,0,0,0,1,0},
			{1,0,0,0,0,0,0,1},
			{0,1,0,0,0,1,1,1}
			};
			int generation = 16;
			int[,] expected = GetGeneration(cells, generation);
			for (int i = 0; i < expected.GetLength(0); i++)
			{
				for (int j = 0; j < expected.GetLength(1); j++)
				{
					Console.Write(expected[i, j] + " ");
				}
				Console.WriteLine();
			}
		}
		public static int[,] GetGeneration(int[,] cells, int generation)
		{
			int[,] expected = new int[cells.GetLength(0), cells.GetLength(1)];
			int[,] expansionBoard = new int[cells.GetLength(0), cells.GetLength(1)];
			int[,] board = new int[cells.GetLength(0), cells.GetLength(1)];
			int countGeneration = 0;
			expansionBoard = cells;
			//En caso de que no nos pasen ninguna generación devolvemos el array tal cual nos ha llegado.
			if (generation == 0)
			{
				return cells;
			}
			//Si la generacion es tan solo 1, me pasa el primer test sin ningún tipo de problema.
			if (generation == 1)
			{
				for (int i = 0; i < expansionBoard.GetLength(0); i++)
				{
					for (int j = 0; j < expansionBoard.GetLength(1); j++)
					{
						if (i == 0 && j == 0) //Esquina superior izquierda [0,0]
						{
							expected[i, j] = ValueCornerTopLeftCells(cells, i, j);
						}

						if (i == expansionBoard.GetLength(0) - 1 && j == 0) // Esquina inferior izquierda (en este caso [2,0] - [ultima fila del array,0])
						{
							expected[i, j] = ValueCornerDownLeftCells(cells, i, j);
						}

						if (i == 0 && j == expansionBoard.GetLength(1) - 1) // Esquina superior derecha (en este caso [0,2] - [0,ultima columna del array])
						{
							expected[i, j] = ValueCornerTopRightCells(cells, i, j);
						}

						if (i == expansionBoard.GetLength(0) - 1 && j == expansionBoard.GetLength(1) - 1) // Esquina inferior derecha (en este caso [2,2] - [ultima fila,ultima columna])
						{
							expected[i, j] = ValueCornerDownRightCells(cells, i, j);
						}

						if (i == 0 && j != expansionBoard.GetLength(1) - 1 && i == 0 && j != 0)
						{
							expected[i, j] = ValueTopDifferentCorners(cells, i, j); // Valor de la linea superior quitando las esquinas [0,1] en este caso [0, todas las columnas entre 0 y la ultima]
						}

						if (i == expansionBoard.GetLength(0) - 1 && j != 0 && i == expansionBoard.GetLength(0) - 1 && j != expansionBoard.GetLength(1) - 1)
						{
							expected[i, j] = ValueBottomDifferentCorners(cells, i, j); // Valor de la linea inferior quitando las esquinas [2,1] en este caso
						}

						if (i != 0 && j == 0 && i != expansionBoard.GetLength(0) - 1 && j == 0)
						{
							expected[i, j] = ValueLeftDifferentCorners(cells, i, j);
						}

						if (i != 0 && j == expansionBoard.GetLength(1) - 1 && i != expansionBoard.GetLength(0) - 1 && j == expansionBoard.GetLength(1) - 1)
						{
							expected[i, j] = ValueRightDifferentCorners(expansionBoard, i, j);
						}

						if (i != 0 && j != 0 && i != expansionBoard.GetLength(0) - 1 && j != expansionBoard.GetLength(1) - 1)
						{
							expected[i, j] = ValueMidBoardDifferentCornersAndDifferentSides(cells, i, j);
						}
					}
				}
				return expected;
			}


			board = cells;
			while (countGeneration < generation)
			{
				board = expansionBoard;
				//Añadimos 2 filas y columnas al inicio de la generacion.
				expansionBoard = new int[expansionBoard.GetLength(0) + 2, expansionBoard.GetLength(1) + 2];
				//Recorremos el array lleno de 0 para implementar los valores del otro array en medio del tablero.
				for (int i = 0; i < expansionBoard.GetLength(0); i++)
				{
					for (int j = 0; j < expansionBoard.GetLength(1); j++)
					{
						//Comprobamos que se encuentren en el centro del array, fuera de las esquinas y de la primera y ultima fila.
						if (i != 0 && j != 0 && i != expansionBoard.GetLength(0) - 1 && j != expansionBoard.GetLength(1) - 1)
						{
							expansionBoard[i, j] = board[i - 1, j - 1]; //En expansionBoard en la primera iteración, cuando lleguemos a [1,1] - copiamos el valor de board[0,0].
						}
					}
				}

				board = new int[expansionBoard.GetLength(0), expansionBoard.GetLength(1)]; //Agrandamos el contenido de Board para que sea el mismo que expansion. Y se llena de 0.

				/*Rellenamos el array de board con el contenido de expansionBoard, aplicando las reglas de ConwayLife.
				 De esta forma rellenamos el array board pasando el contenido de expansionBoard.*/
				for (int i = 0; i < board.GetLength(0); i++)
				{
					for (int j = 0; j < board.GetLength(1); j++)
					{
						if (i == 0 && j == 0) //Esquina superior izquierda [0,0]
						{
							board[i, j] = ValueCornerTopLeftCells(expansionBoard, i, j);
						}

						if (i == expansionBoard.GetLength(0) - 1 && j == 0) // Esquina inferior izquierda (en este caso [2,0] )
						{
							board[i, j] = ValueCornerDownLeftCells(expansionBoard, i, j);
						}

						if (i == 0 && j == expansionBoard.GetLength(1) - 1) // Esquina superior derecha (en este caso [0,7])
						{
							board[i, j] = ValueCornerTopRightCells(expansionBoard, i, j);
						}

						if (i == expansionBoard.GetLength(0) - 1 && j == expansionBoard.GetLength(1) - 1) // Esquina inferior derecha (en este caso [2,7])
						{
							board[i, j] = ValueCornerDownRightCells(expansionBoard, i, j);
						}

						if (i == 0 && j != expansionBoard.GetLength(1) - 1 && i == 0 && j != 0)
						{
							board[i, j] = ValueTopDifferentCorners(expansionBoard, i, j); // Valor de la linea superior quitando las esquinas en este caso desde [0,1] hasta [0,n-1]
						}

						if (i == expansionBoard.GetLength(0) - 1 && j != 0 && i == expansionBoard.GetLength(0) - 1 && j != expansionBoard.GetLength(1) - 1) // Valor de la linea inferior quitando las esquinas en este caso [2,1] hasta [2,n-1];
						{
							board[i, j] = ValueBottomDifferentCorners(expansionBoard, i, j);
						}

						if (i != 0 && j == 0 && i != expansionBoard.GetLength(0) - 1 && j == 0) // Valores para [1,0] hasta [n-1,0]
						{
							board[i, j] = ValueLeftDifferentCorners(expansionBoard, i, j);
						}

						if (i != 0 && j == expansionBoard.GetLength(1) - 1 && i != expansionBoard.GetLength(0) - 1 && j == expansionBoard.GetLength(1) - 1)// Valores para la ultima columna
						{
							board[i, j] = ValueRightDifferentCorners(expansionBoard, i, j);
						}

						if (i != 0 && j != 0 && i != expansionBoard.GetLength(0) - 1 && j != expansionBoard.GetLength(1) - 1) // Valores para medio del tablero
						{
							board[i, j] = ValueMidBoardDifferentCornersAndDifferentSides(expansionBoard, i, j);
						}
					}
				}
				expansionBoard = board;
				/*A continuación, trabajamos sobre el array Board para observar la primera y ultima fila junto con la primera y ultima columna.
				 En caso de que no exista ninguna celula viva (valor1) se eliminan.*/
				countGeneration++;
			}

			bool reduced;
			do
			{
				reduced = false;
				int countColumnFirst = 0;
				int countColumnLast = 0;
				int countRowUp = 0;
				int countRowDown = 0;

				// Verificar si la primera o última fila está llena de ceros
				for (int i = 0; i < board.GetLength(0); i++)
				{
					if (board[i, 0] == 0) countRowUp++;
					if (board[i, board.GetLength(1) - 1] == 0) countRowDown++;
				}

				// Verificar si la primera o última columna está llena de ceros
				for (int j = 0; j < board.GetLength(1); j++)
				{
					if (board[0, j] == 0) countColumnFirst++;
					if (board[board.GetLength(0) - 1, j] == 0) countColumnLast++;
				}

				// Eliminar filas si están llenas de ceros
				if (countRowUp == board.GetLength(0))
				{
					// Reduce el tamaño del array temporalmente
					int[,] temporalArray = new int[board.GetLength(0) - 1, board.GetLength(1)];
					for (int i = 1; i < board.GetLength(0); i++)
					{
						for (int j = 0; j < board.GetLength(1); j++)
						{
							if (i >= 0 && i < board.GetLength(0) && j >= 0 && j < board.GetLength(1))
							{
								temporalArray[i - 1, j] = board[i, j];
							}
						}
					}
					board = temporalArray;
					reduced = true;
				}
				if (countRowDown == board.GetLength(0))
				{
					int[,] temporalArray = new int[board.GetLength(0) - 1, board.GetLength(1)];
					for (int i = 0; i < board.GetLength(0) - 1; i++)
					{
						for (int j = 0; j < board.GetLength(1); j++)
						{
							if (i >= 0 && i < board.GetLength(0) && j >= 0 && j < board.GetLength(1))
							{
								temporalArray[i, j] = board[i, j];
							}
						}
					}
					board = temporalArray;
					reduced = true;
				}

				// Eliminar columnas si están llenas de ceros
				if (countColumnFirst == board.GetLength(1))
				{
					int[,] temporalArray = new int[board.GetLength(0), board.GetLength(1) - 1];
					for (int i = 0; i < board.GetLength(0); i++)
					{
						for (int j = 1; j < board.GetLength(1); j++)
						{
							if (i >= 0 && i < board.GetLength(0) && j >= 0 && j < board.GetLength(1))
							{
								temporalArray[i, j - 1] = board[i, j];
							}
						}
					}
					board = temporalArray;
					reduced = true;
				}
				if (countColumnLast == board.GetLength(1))
				{
					int[,] temporalArray = new int[board.GetLength(0), board.GetLength(1) - 1];
					for (int i = 0; i < board.GetLength(0); i++)
					{
						for (int j = 0; j < board.GetLength(1) - 1; j++)
						{
							if (i >= 0 && i < board.GetLength(0) && j >= 0 && j < board.GetLength(1))
							{
								temporalArray[i, j] = board[i, j];
							}
						}
					}
					board = temporalArray;
					reduced = true;
				}

			} while (reduced);
			return board;
		}
		private static int CountNeighbors(int[,] board, int row, int col)
		{
			int count = 0;
			int rows = board.GetLength(0);
			int cols = board.GetLength(1);

			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i == 0 && j == 0) continue; // Ignora la célula misma
					int newRow = row + i;
					int newCol = col + j;
					if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
					{
						count += board[newRow, newCol];
					}
				}
			}
			return count;
		}

		public static int ValueCornerTopLeftCells(int[,] expansionBoard, int valueI, int valueJ)
		{


			int rightValue = expansionBoard[valueI, valueJ + 1];
			int crossValue = expansionBoard[valueI + 1, valueJ + 1];
			int downValue = expansionBoard[valueI + 1, valueJ];
			int actualValue = expansionBoard[valueI, valueJ];
			int countHp = 0;

			if (rightValue == 1) countHp++;
			if (crossValue == 1) countHp++;
			if (downValue == 1) countHp++;

			if (actualValue == 1 && countHp < 2 || countHp > 3) actualValue = 0;
			else if (actualValue == 1 && countHp == 2 || countHp == 3) actualValue = 1;

			if (actualValue == 0 && countHp == 3) actualValue = 1;
			return actualValue;


		}
		public static int ValueCornerDownLeftCells(int[,] expansionBoard, int valueI, int valueJ)
		{

			int upValue = expansionBoard[valueI - 1, valueJ];
			int crossValue = expansionBoard[valueI - 1, valueJ + 1];
			int rightValue = expansionBoard[valueI, valueJ + 1];
			int countHp = 0;
			int actualValue = expansionBoard[valueI, valueJ];

			if (rightValue == 1) countHp++;
			if (crossValue == 1) countHp++;
			if (upValue == 1) countHp++;

			if (actualValue == 1 && countHp < 2 || countHp > 3) actualValue = 0;
			else if (actualValue == 1 && countHp == 2 || countHp == 3) actualValue = 1;

			if (actualValue == 0 && countHp == 3) actualValue = 1;
			return actualValue;

		}
		public static int ValueCornerTopRightCells(int[,] expansionBoard, int valueI, int valueJ)
		{
			int leftValue = expansionBoard[valueI, valueJ - 1];
			int crossValue = expansionBoard[valueI + 1, valueJ - 1];
			int downValue = expansionBoard[valueI + 1, valueJ];
			int actualValue = expansionBoard[valueI, valueJ];
			int countHp = 0;

			if (leftValue == 1) countHp++;
			if (crossValue == 1) countHp++;
			if (downValue == 1) countHp++;

			if (actualValue == 1 && countHp < 2 || countHp > 3) actualValue = 0;
			else if (actualValue == 1 && countHp == 2 || countHp == 3) actualValue = 1;

			if (actualValue == 0 && countHp == 3) actualValue = 1;
			return actualValue;
		}
		public static int ValueCornerDownRightCells(int[,] expansionBoard, int valueI, int valueJ)
		{

			int upValue = expansionBoard[valueI - 1, valueJ];
			int crossValue = expansionBoard[valueI - 1, valueJ - 1];
			int leftValue = expansionBoard[valueI, valueJ - 1];
			int actualValue = expansionBoard[valueI, valueJ];
			int countHp = 0;

			if (leftValue == 1) countHp++;
			if (crossValue == 1) countHp++;
			if (upValue == 1) countHp++;

			if (actualValue == 1 && countHp < 2 || countHp > 3) actualValue = 0;
			else if (actualValue == 1 && countHp == 2 || countHp == 3) actualValue = 1;

			if (actualValue == 0 && countHp == 3) actualValue = 1;
			return actualValue;

		}

		public static int ValueTopDifferentCorners(int[,] expansionBoard, int valueI, int valueJ)
		{

			int leftValue = expansionBoard[valueI, valueJ - 1];
			int crossLeftValue = expansionBoard[valueI + 1, valueJ - 1];
			int donwValue = expansionBoard[valueI + 1, valueJ];
			int crossRightValue = expansionBoard[valueI + 1, valueJ + 1];
			int rightValue = expansionBoard[valueI, valueJ + 1];
			int actualValue = expansionBoard[valueI, valueJ];
			int countHp = 0;

			if (leftValue == 1) countHp++;
			if (crossLeftValue == 1) countHp++;
			if (donwValue == 1) countHp++;
			if (crossRightValue == 1) countHp++;
			if (rightValue == 1) countHp++;

			if (actualValue == 1 && countHp < 2 || countHp > 3) actualValue = 0;
			else if (actualValue == 1 && countHp == 2 || countHp == 3) actualValue = 1;

			if (actualValue == 0 && countHp == 3) actualValue = 1;
			return actualValue;
		}

		public static int ValueBottomDifferentCorners(int[,] expansionBoard, int valueI, int valueJ)
		{

			int leftValue = expansionBoard[valueI, valueJ - 1];
			int crossLeftValue = expansionBoard[valueI - 1, valueJ - 1];
			int upValue = expansionBoard[valueI - 1, valueJ];
			int crossRightValue = expansionBoard[valueI - 1, valueJ + 1];
			int rightValue = expansionBoard[valueI, valueJ + 1];

			int actualValue = expansionBoard[valueI, valueJ];
			int countHp = 0;

			if (leftValue == 1) countHp++;
			if (crossLeftValue == 1) countHp++;
			if (upValue == 1) countHp++;
			if (crossRightValue == 1) countHp++;
			if (rightValue == 1) countHp++;

			if (actualValue == 1 && countHp < 2 || countHp > 3) actualValue = 0;
			else if (actualValue == 1 && countHp == 2 || countHp == 3) actualValue = 1;

			if (actualValue == 0 && countHp == 3) actualValue = 1;
			return actualValue;
		}

		public static int ValueLeftDifferentCorners(int[,] expansionBoard, int valueI, int valueJ)
		{

			int upValue = expansionBoard[valueI - 1, valueJ];
			int crossUpValue = expansionBoard[valueI - 1, valueJ + 1];
			int rightValue = expansionBoard[valueI, valueJ + 1];
			int crossDownValue = expansionBoard[valueI + 1, valueJ + 1];
			int downValue = expansionBoard[valueI + 1, valueJ];
			int actualValue = expansionBoard[valueI, valueJ];
			int countHp = 0;

			if (upValue == 1) countHp++;
			if (crossUpValue == 1) countHp++;
			if (rightValue == 1) countHp++;
			if (crossDownValue == 1) countHp++;
			if (downValue == 1) countHp++;

			if (actualValue == 1 && countHp < 2 || countHp > 3) actualValue = 0;
			else if (actualValue == 1 && countHp == 2 || countHp == 3) actualValue = 1;

			if (actualValue == 0 && countHp == 3) actualValue = 1;

			return actualValue;

		}

		public static int ValueRightDifferentCorners(int[,] expansionBoard, int valueI, int valueJ)
		{
			int upValue = expansionBoard[valueI - 1, valueJ];
			int crossUpLeft = expansionBoard[valueI - 1, valueJ - 1];
			int leftValue = expansionBoard[valueI, valueJ - 1];
			int crossDownLeft = expansionBoard[valueI + 1, valueJ - 1];
			int downValue = expansionBoard[valueI + 1, valueJ];
			int actualValue = expansionBoard[valueI, valueJ];
			int countHp = 0;

			if (upValue == 1) countHp++;
			if (crossUpLeft == 1) countHp++;
			if (leftValue == 1) countHp++;
			if (crossDownLeft == 1) countHp++;
			if (downValue == 1) countHp++;


			if (actualValue == 1 && countHp < 2 || countHp > 3) actualValue = 0;
			else if (actualValue == 1 && countHp == 2 || countHp == 3) actualValue = 1;

			if (actualValue == 0 && countHp == 3) actualValue = 1;

			return actualValue;

		}

		public static int ValueMidBoardDifferentCornersAndDifferentSides(int[,] expansionBoard, int valueI, int valueJ)
		{

			int crossTopLeft = expansionBoard[valueI - 1, valueJ - 1];
			int topSide = expansionBoard[valueI - 1, valueJ];
			int crossTopRight = expansionBoard[valueI - 1, valueJ + 1];

			int leftSide = expansionBoard[valueI, valueJ - 1];
			int rightSide = expansionBoard[valueI, valueJ + 1];

			int crossDownLeft = expansionBoard[valueI + 1, valueJ - 1];
			int downSide = expansionBoard[valueI + 1, valueJ];
			int crossDownRight = expansionBoard[valueI + 1, valueJ + 1];

			int actualValue = expansionBoard[valueI, valueJ];
			int countHp = 0;

			if (crossTopLeft == 1) countHp++;
			if (topSide == 1) countHp++;
			if (crossTopRight == 1) countHp++;
			if (leftSide == 1) countHp++;
			if (rightSide == 1) countHp++;
			if (crossDownLeft == 1) countHp++;
			if (downSide == 1) countHp++;
			if (crossDownRight == 1) countHp++;

			if (actualValue == 1 && countHp < 2 || countHp > 3) actualValue = 0;
			else if (actualValue == 1 && countHp == 2 || countHp == 3) actualValue = 1;

			if (actualValue == 0 && countHp == 3) actualValue = 1;

			return actualValue;
		}
	}
}