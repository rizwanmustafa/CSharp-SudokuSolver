using System;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
		// Intitialize a 3x3 board. You can change the size by modifying the parameter value
		var myBoard = new SudokuBoard(3);
		// Assign custom values by inputting them
		myBoard.AssignValues();
		// Print the unsolved board
		myBoard.PrintBoard();
		// Solve the board and print it
		myBoard.SolveBoard();
        }
    }
}
