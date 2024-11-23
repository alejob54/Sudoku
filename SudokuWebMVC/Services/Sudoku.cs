using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SudokuWebMVC.Enum;
using System.Threading.Tasks;
using SudokuWebMVC.Validations;
using SudokuWebMVC.Models;

namespace SudokuWebMVC.Services
{
    public class Sudoku
    {

        public async Task GenerateRandom(int Method)
        {
            int[,] matrix = new int[9, 9];
            bool isValid = false;
            DateTime start = DateTime.UtcNow;

            if (Method.Equals(5))
            {
                await new SudokuGenerator().RandomizeFromFolderAsync().ConfigureAwait(false);
                Console.WriteLine("Process Finished");
            }
            else
            {
                while (true)
                {
                    matrix = await new SudokuGenerator().LoadRandom(Method).ConfigureAwait(false);

                    isValid = await new SudokuValidations().MatrixIsDoneAsync(matrix).ConfigureAwait(false);

                    if (isValid)
                    {
                        var end = DateTime.UtcNow;
                        Console.WriteLine($"Found in {end.Subtract(start).TotalMinutes} minutes");
                        await PrintMatrix(matrix).ConfigureAwait(false);
                        await new SudokuGenerator().SaveAsync(matrix).ConfigureAwait(false);
                        start = DateTime.UtcNow;
                    }
                }
            }
        }

        public async Task PrintMatrix(int[,] matrix)
        {
            await Task.Run(() =>
            {
                for (int x = 0; x < matrix.GetLongLength(0); x++)
                {
                    for (int y = 0; y < matrix.GetLongLength(1); y++)
                    {
                        if (matrix[x, y].Equals(0))
                        {
                            Console.Write(" ");
                        }
                        else
                        {
                            Console.Write(matrix[x, y]);
                        }
                    }
                    Console.WriteLine("");
                }
            }).ConfigureAwait(false);
        }
    }
}
