using System;
using System.Threading.Tasks;
using SudokuWebMVC.Enum;
using SudokuWebMVC.Helpers;
using SudokuWebMVC.Services;
using SudokuWebMVC.Validations;

namespace SudokuTester
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
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
                    await TestGenerateRandomSudokuAsync().ConfigureAwait(false);
                    break;
                case 2:
                    await ValidateFolderAsync().ConfigureAwait(false);
                    break;
                case 3:
                    await GetNewBoardAsync().ConfigureAwait(false);
                    break;
                case 4:
                    await TestGenerateRandomSudoku3D().ConfigureAwait(false);
                    break;
            }

            Console.Read();
        }

        static async Task TestGenerateRandomSudokuAsync()
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
            await sudoku.GenerateRandomAsync(Method).ConfigureAwait(false);
        }

        static async Task ValidateFolderAsync()
        {
            Console.WriteLine("1. Validate");
            Console.WriteLine("2. Validate and delete invalid files");
            int input = int.Parse(Console.ReadLine());
            Console.WriteLine("Path to folder: ");
            string path = Console.ReadLine();

            SudokuValidator sudokuValidator = new SudokuValidator();
            await sudokuValidator.ValidateFolderAsync(path, input == 2).ConfigureAwait(false);
        }

        static async Task GetNewBoardAsync()
        {
            Console.WriteLine("Choose Difficulty:");
            Console.WriteLine("1. Easy");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. Hard");
            int Dif = int.Parse(Console.ReadLine());
            var NewBoard = await new SudokuGenerator().LoadFromFileAsync().ConfigureAwait(false);
            NewBoard = await new SudokuGenerator().PrepareBoardAsync((Difficulty)Dif, NewBoard).ConfigureAwait(false);
            Console.WriteLine("---------");
            await new Sudoku().PrintMatrixAsync(NewBoard).ConfigureAwait(false);
            Console.WriteLine("---------");
        }

        static async Task TestGenerateRandomSudoku3D()
        {
            await Task.Run(() =>
            {
                //It'll run until you close the program
                Sudoku sudoku = new Sudoku();
                Console.WriteLine("Select method: ");
                Console.WriteLine("1.By adding inner cubes (27)");

                Sudoku3D sudoku3D = new Sudoku3D();
                sudoku3D.Create3DCube();
            }).ConfigureAwait(false);
        }
    }
}
