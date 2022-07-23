using System;
using SudokuWebMVC.Helpers;

namespace SudokuTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("*************************************************************************");
            Console.WriteLine("***************************** SUDOKU TESTER *****************************");
            Console.WriteLine("*************************************************************************");

            Console.WriteLine("1. Generate Sudokus");
            Console.WriteLine("2. Validate Folder");
            int input = int.Parse(Console.ReadLine());
            if (input == 1)
            {
                TestGenerateRandomSudoku();
            }
            else if (input == 2)
            {
                ValidateFolder();
            }

            Console.ReadKey();
        }

        static void TestGenerateRandomSudoku()
        {
            //It'll run until you close the program
            Sudoku sudoku = new Sudoku();
            Console.WriteLine("Select ordering method: ");
            Console.WriteLine("1. Ordered (1-2-3-4-5-6-7-8-9)");
            Console.WriteLine("2. Cross (4-5-6-2-8-1-3-7-9)");
            Console.WriteLine("3. Spiral (1-2-3-6-9-8-7-4-5) ** Slightly Faster");
            Console.WriteLine("4. Spiral Inverted (5-6-3-2-1-4-7-8-9)");
            Console.WriteLine("5. *** Randomize from existing folder ***");

            int Method = int.Parse(Console.ReadLine());
            sudoku.GenerateRandom(Method);
        }

        static void ValidateFolder()
        {
            Console.WriteLine("1. Validate");
            Console.WriteLine("2. Validate and delete invalid files");
            int input = int.Parse(Console.ReadLine());

            SudokuValidator sudokuValidator = new SudokuValidator();
            sudokuValidator.ValidateFolder(@"C:/sudoku/", input == 2);
        }
    }
}
