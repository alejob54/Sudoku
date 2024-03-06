using System;
using SudokuWebMVC.Enum;
using SudokuWebMVC.Helpers;

namespace SudokuTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("*************************************************************************");
            Console.WriteLine("***************************** SUDOKU TESTER *****************************");
            Console.WriteLine("*************************************************************************");

            Console.WriteLine("1. Generate Sudokus 2D");
            Console.WriteLine("2. Validate Folder 2D");
            Console.WriteLine("3. Get new board 2D");
            Console.WriteLine("4. Create new board 3D");
            int input = int.Parse(Console.ReadLine());
            switch (input)
            {
                case 1:
                    TestGenerateRandomSudoku();
                    break;
                case 2:
                    ValidateFolder();
                    break;
                case 3:
                    GetNewBoard();
                    break;
                case 4:
                    TestGenerateRandomSudoku3D();
                    break;
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
            Console.WriteLine("Path to folder: ");
            string path = Console.ReadLine();

            SudokuValidator sudokuValidator = new SudokuValidator();
            sudokuValidator.ValidateFolder(path, input == 2);
           
        }

        static void GetNewBoard()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Choose Difficulty:");
            Console.WriteLine("1. Easy");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. Hard");
            Console.ForegroundColor = ConsoleColor.White;
            int Dif = int.Parse(Console.ReadLine());
            var NewBoard = new SudokuGenerator().LoadFromFile();
            NewBoard = new SudokuGenerator().PrepareBoard((Difficulty)Dif, NewBoard);
            Console.WriteLine("---------");
            new Sudoku().PrintMatrix(NewBoard);
            Console.WriteLine("---------");
        }

        static void TestGenerateRandomSudoku3D()
        {
            //It'll run until you close the program
            Sudoku sudoku = new Sudoku();
            Console.WriteLine("Select method: ");
            Console.WriteLine("1.By adding inner cubes (27)");
            
            Sudoku3D sudoku3D = new Sudoku3D();
            sudoku3D.Create3DCube();
        }
    }
}
