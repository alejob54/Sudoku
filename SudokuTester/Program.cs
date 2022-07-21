using System;
using SudokuWebMVC.Helpers;

namespace SudokuTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestGenerateRandomSudoku();
            Console.ReadKey();
        }

        static void TestGenerateRandomSudoku()
        {
            //It'll run until you close the program
            Sudoku sudoku = new Sudoku();
            Console.WriteLine("Select ordering method: ");
            Console.WriteLine("1. Ordered (1-2-3-4-5-6-7-8-9)");
            Console.WriteLine("2. Cross (4-5-6-2-8-1-3-7-9)");
            Console.WriteLine("3. Spiral (1-2-3-6-9-8-7-4-5)");
            Console.WriteLine("4. Spiral Inverted (5-6-3-2-1-4-7-8-9)");

            int Method = int.Parse(Console.ReadLine());
            sudoku.GenerateRandom(Method);
        }
    }
}
