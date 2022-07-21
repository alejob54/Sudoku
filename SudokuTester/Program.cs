using System;
using SudokuWebMVC.Helpers;

namespace SudokuTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TestGenerateSudoku();
            TestGenerateRandomSudoku();
            Console.ReadKey();
        }

        static void TestGenerateSudoku()
        {
            Sudoku sudoku = new Sudoku();
            var result = sudoku.Generate();

            for (int x = 0; x < result.GetLongLength(0); x++)
            {
                for (int y = 0; y < result.GetLongLength(1); y++)
                {
                    Console.Write(result[x, y]);
                }
                Console.WriteLine("");
            }
        }

        static void TestGenerateRandomSudoku()
        {
            Sudoku sudoku = new Sudoku();
            var result = sudoku.GenerateRandom();
        }
    }
}
