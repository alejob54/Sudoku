using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuWebMVC.Validations;

public class SudokuValidator
{
    public async Task ValidateFolderAsync(string path, bool deleteInvalidFiles = false)
    {
        var files = Directory.GetFiles(path);
        if (files.Count() == 0)
        {
            Console.WriteLine($"No Files found for given path: {path}");
            return;
        }
        int i = 1;
        foreach (var item in files)
        {
            var extension = new FileInfo(item).Extension;
            if (extension != ".txt") continue;
            int[,] Matrix = await ConvertFileToMatrixAsync(File.ReadAllLines(item)).ConfigureAwait(false);
            bool IsValid = await new SudokuValidations().MatrixIsDoneAsync(Matrix).ConfigureAwait(false);
            if (IsValid)
            {
                Console.WriteLine($"{i} - {item} is valid");
            }
            else
            {
                Console.WriteLine($"{i} - {item} is invalid");

                if (deleteInvalidFiles)
                {
                    File.Delete(item);
                }
            }
            i++;
        }
    }

    public async Task<int[,]> ConvertFileToMatrixAsync(string[] content)
    {
        return await Task.Run(() =>
        {
            int[,] matrix = new int[9, 9];
            int x = 0;

            foreach (string item in content)
            {
                for (int y = 0; y < 9; y++)
                {
                    matrix[x, y] = Convert.ToInt32(item[y].ToString());
                }
                x++;
            }

            return matrix;
        }).ConfigureAwait(false);
    }
}
