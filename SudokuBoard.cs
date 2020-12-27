using System;
using System.Collections.Generic;

namespace SudokuSolver
{

	class SudokuBoard
	{

		// The length of a box in the board
		int minLength;
		// The complete length of the board
		int maxLength;
		// Multi-dimensional array to store the values of the board
		int[,] boardArray;

		public SudokuBoard(int boxLength)
		{
			// We take the box length as a parameter to prevent taking the square root which can be slow on larger boards
			minLength = boxLength;
			maxLength = boxLength * boxLength;

			// Print the statistics of the board
			Console.WriteLine($"Min Length: {minLength}");
			Console.WriteLine($"Max Length: {maxLength}");

			// Assign the array
			boardArray = new int[maxLength, maxLength];
		}

		public void PrintBoard()
		{
			// If the board has not been assigned, do nothing
			if (boardArray == null || boardArray.Length == 0) return;

			for (int row = 0; row < maxLength; row++)
			{
				// This prints a dashed line before the first row of every box 
				if (row % minLength == 0)
					Console.WriteLine(new string('_', 2 * maxLength + 3 + (minLength - 1) * 2) + Environment.NewLine);

				// This holds the data to be printed and will be printed once at the end of this row
				string printLine = "";

				for (int col = 0; col < maxLength; col++)
				{
					// This adds a separator character before the first column of box
					if (col % minLength == 0)
						printLine += "| ";
					// This adds the character and a spacing after it to distinguish it from the next character
					printLine += $"{boardArray[row, col]} ";
				}
				// Add the ending separator
				printLine += "|";
				// Finally, print the row data
				Console.WriteLine(printLine);
			}

			// Print the final dashed line to mark the end of the board
			Console.WriteLine(new string('_', 2 * maxLength + 3 + (minLength - 1) * 2) + Environment.NewLine);
		}

		public void AssignValues()
		{
			// The following code will allow the user to enter values to the board. Empty slots should be denoted as 0
			// Each user input should look something like this: 'Please enter the values for row 1: 1 2 3 4 5 6 7 8 9'
			// Every character should be a number and there must be a space between each number		
			for (int row = 0; row < maxLength; row++)
			{
				Console.Write($"Please enter the values for row {row + 1}: ");
				// We split the input string by every space and then assign the first character of every string to the board
				string[] myNums = Console.ReadLine().Split(' ');

				for (int col = 0; col < maxLength; col++)
				{
					myNums[col] = myNums[col][0].ToString();
					boardArray[row, col] = int.Parse(myNums[col]);
				}
			}
		}

		public void SolveBoard()
		{
			Console.WriteLine("Searching for a solution!");
			// Intialize the row and col variables to store the current row and column respectively
			int row = -1, col = -1;
			// Initialize a stack to store the cells of the board we modified, for accessing them in case we have to backtrack
			Stack<Cell> changedCells = new Stack<Cell>();
			// Assign the values of the first empty cell we could find
			GetEmptyCellReference(out row, out col);

			// Keep iterating until all empty cells have been filled and the solution works
			while (row != -1 && col != -1)
			{
				// If the cell we are working on has a value, increase that value by 1 and move on otherwise set 1 as the value 
				int supposedNum = boardArray[row,col] == 0? 1: boardArray[row,col] + 1;
				// Increment the potential value until it is a valid value for the board or it has no possible solution 
				while (supposedNum < 10)
				{
					if (IsValidNum(supposedNum, row, col)) break;
					else supposedNum++;
				}

				if (supposedNum > 9)
				{
					// If we have the option to back track do it or else just say that there is no solution
					if (changedCells.Count == 0)
					{
						Console.WriteLine("No Possible Solution Found!");
						return;
					}
					else
					{
						// Set the value of current cell as 0 and move on to the last cell we edited
						boardArray[row,col] = 0;
						Cell lastEditedCell = changedCells.Pop();
						row = lastEditedCell.row;
						col = lastEditedCell.col;
						continue;
					}
				}
				else
				{
					// Assign the potential value to the board and push it to stack for future backtracking
					boardArray[row, col] = supposedNum;
					changedCells.Push(new Cell(row, col));
					// Assign the co-ordinates of an empty cell to the variable
					GetEmptyCellReference(out row, out col);
				}
			}

			// Now that there is no empty cell and the board has been solved. Print it 
			Console.WriteLine("Found a solution");
			PrintBoard();
		}

		public void GetEmptyCellReference(out int row, out int col)
		{
			// Iterate through each of the cell in the board and if an empty cell is spotted, assign its co-ordinates and return.
			// In case, no empty cell is found assign the values as -1 and return.
			for (int a = 0; a < maxLength; a++)
			{
				for (int b = 0; b < maxLength; b++)
				{
					if (boardArray[a, b] == 0)
					{
						row = a;
						col = b;
						return;
					}
				}
			}
			row = -1;
			col = -1;
		}

		public bool IsValidNum(int val, int row, int col)
		{
			for (int a = 0; a < maxLength; a++)
			{
				for (int b = 0; b < maxLength; b++)
				{
					if (boardArray[a, b] == val)
					{
						// Check if the similar value is in the same row, column or even box 

						if (a == row || b == col) return false;
						else
						{
							int minRow = 0, maxRow = 0, minCol = 0, maxCol = 0;
							int minRow2 = 0, maxRow2 = 0, minCol2 = 0, maxCol2 = 0;
							// Get the dimensions of the boxes of the similar value and the current cell 
							GetBoxDimensions(a, b, out minRow, out maxRow, out minCol, out maxCol);
							GetBoxDimensions(row, col, out minRow2, out maxRow2, out minCol2, out maxCol2);

							// If the dimensions of the boxes are same, it means both are in the same box
							if (minRow == minRow2 && maxCol == maxCol2) return false;
						}
					}
				}
			}
			return true;
		}

		public void GetBoxDimensions(int row, int col, out int minRow, out int maxRow, out int minCol, out int maxCol)
		{
			// Get the index of starting and ending row of the box respectively
			minRow = Convert.ToInt32((row / minLength) * minLength);
			maxRow = minRow + minLength - 1;
			// Get the index of starting and ending column of the box respectively
			minCol = Convert.ToInt32((col / minLength) * minLength);
			maxCol = minCol + minLength - 1;
		}

		// A custom datatype for holding the co-ordinates of a cell in the board
		public struct Cell
		{
			public int row;
			public int col;

			public Cell(int row, int col)
			{
				this.row = row;
				this.col = col;
			}
		}
	}
}
