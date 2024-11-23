using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuWebMVC.Enum;
using SudokuWebMVC.Validations;

namespace SudokuWebMVC.Services;

public class SudokuGenerator
{
    public class BoardRandomizer
    {
        public int Number { get; set; }
        public int ReplacedNumber { get; set; }
    }
    public async Task<int[,]> LoadRandomAsync(int Method)
    {
        var finalMatrix = new int[9, 9];
        List<SudokuOrderForAdding> sudokuOrderForAdding = new List<SudokuOrderForAdding>();
        switch (Method)
        {
            case 1:
                sudokuOrderForAdding = new SudokuOrderForAdding().GetSudokuOrderForAdding_OrderedMethod();
                break;
            case 2:
                sudokuOrderForAdding = new SudokuOrderForAdding().GetSudokuOrderForAdding_CrossMethod();
                break;
            case 3:
                sudokuOrderForAdding = new SudokuOrderForAdding().GetSudokuOrderForAdding_SpiralMethod();
                break;
            case 4:
                sudokuOrderForAdding = new SudokuOrderForAdding().GetSudokuOrderForAdding_SpiralInvertedMethod();
                break;
            default:
                return default;
        }
        int maxAttempts = (9 * 8 * 7 * 6 * 5 * 4 * 3 * 2 * 1);
        int currentLastAttempt = 1;
        while (!await new SudokuValidations().MatrixIsDoneAsync(finalMatrix))
        {
            if (currentLastAttempt > maxAttempts)
            {
                Console.WriteLine("Reaching limit of combinations --- > Skipping");
                break;
            }
            var CurrentOrder = sudokuOrderForAdding.Where(a => !a.Done).Take(1).First();
            var TemporalMatrix = await GetTentativeValuesAsync().ConfigureAwait(false);
            var FinalMatrixCopy = finalMatrix;

            FinalMatrixCopy = await AddToMatrixAsync(FinalMatrixCopy, TemporalMatrix, CurrentOrder);

            //Validate. 
            if (await ValidateTemporalAddingAsync(FinalMatrixCopy).ConfigureAwait(false))
            {
                finalMatrix = FinalMatrixCopy;
                //this group has passed the validations.
                sudokuOrderForAdding[CurrentOrder.Order - 1].Done = true;
                Console.WriteLine("Group Added " + CurrentOrder.Value + " after " + currentLastAttempt + " attempts");
                currentLastAttempt = 1;
            }

            currentLastAttempt++;
        }

        return finalMatrix;
    }

    /// <summary>
    /// Loads a valid board from file
    /// </summary>
    /// <returns></returns>
    public async Task<int[,]> LoadFromFileAsync()
    {
        string Path = @"/PathToFolder/";

        try
        {
            var Files = Directory.GetFiles(Path);
            int Random = new Random().Next(0, Files.Count());
            return await new SudokuValidator().ConvertFileToMatrixAsync(File.ReadAllLines(Files[Random])).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var det = ex.Message;
            throw new Exception();
        }
    }

    /// <summary>
    /// validate this group of data is valid according to its row and colum
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public async Task<bool> ValidateTemporalAddingAsync(int[,] matrix)
    {
        var Validations = new SudokuValidations();

        //rows
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            var array = await Validations.GetRowAsync(matrix, i).ConfigureAwait(false);
            if (!await Validations.ValidateDuplicatedTemporalValuesInArrayAsync(array).ConfigureAwait(false))
            {
                return false;
            }
        }

        //columns
        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            var array = await Validations.GetColumnAsync(matrix, i).ConfigureAwait(false);
            if (!await Validations.ValidateDuplicatedTemporalValuesInArrayAsync(array).ConfigureAwait(false))
            {
                return false;
            }
        }

        return true;
    }

    //add matrix piece to another matrix given a specified coordenates of the location.
    // i.e., add section 3 from a matrix A to section 3 of matrix B, being 3 the third upper right quadrant.
    private async Task<int[,]> AddToMatrixAsync(int[,] destination, int[,] origin, SudokuOrderForAdding order)
    {
        return await Task.Run(() =>
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    destination[order.XCoordinate + x, order.YCoordinate + y] = origin[x, y];
                }
            }

            return destination;
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a 3x3 matrix with random data
    /// </summary>
    /// <returns></returns>
    private async Task<int[,]> GetTentativeValuesAsync()
    {
        var TmpMatrix = new int[3, 3];
        var Ints = await GetRandomizedIntegerListAsync().ConfigureAwait(false);
        int i = 0;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                TmpMatrix[x, y] = Ints[i];
                i++;
            }
        }

        return TmpMatrix;
    }

    /// <summary>
    /// Gets an integer list with 9 random rumbers.
    /// </summary>
    /// <returns></returns>
    private async Task<List<int>> GetRandomizedIntegerListAsync()
    {
        return await Task.Run(() =>
        {
            HashSet<int> ints = new HashSet<int>();
            Random random = new Random();

            while (ints.Count < 9)
            {
                int rnd = random.Next(1, 10);
                ints.Add(rnd);
            }

            return ints.ToList();
        }).ConfigureAwait(false);

    }

    /// <summary>
    /// Given a folder with valid sudoku's files, randomizes the content of each one to create 
    /// more valid boards. This will run forever until you stop it or blows your computer up.
    /// </summary>
    public async Task RandomizeFromFolderAsync()
    {
        string FolderPath = @"sudoku/";

        while (true)
        {
            GC.Collect();
            foreach (string item in Directory.GetFiles(FolderPath))
            {
                var extension = new FileInfo(item).Extension;
                if (extension != ".txt") continue;
                //get current board
                var matrix = await new SudokuValidator().ConvertFileToMatrixAsync(File.ReadAllLines(item)).ConfigureAwait(false);
                //validate first this is a valid board
                if (await new SudokuValidations().MatrixIsDoneAsync(matrix))
                {
                    //replace by new values
                    var newMatrix = await ReplaceMatrix(matrix).ConfigureAwait(false);
                    if (await new SudokuValidations().MatrixIsDoneAsync(newMatrix).ConfigureAwait(false))
                    {
                        await SaveAsync(newMatrix).ConfigureAwait(false);
                        Console.WriteLine($"Matrix {item} has been mixed successfully");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Matrix with values replaced</returns>
    private async Task<int[,]> ReplaceMatrix(int[,] inputMatrix)
    {
        //find and replace all current values for new ones,
        //For example, all #1 by 33, all #2 by 77, all #3 by 99 and so on.
        //Those are random generated numbers.

        var BoardRandomizer = await GetBoardRandomizersAsync().ConfigureAwait(false);
        for (int x = 0; x < inputMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < inputMatrix.GetLength(1); y++)
            {
                inputMatrix[x, y] = await FindNewNumberAsync(BoardRandomizer, inputMatrix[x, y]).ConfigureAwait(false);
            }
        }

        //Now that the matrix is all shuffled, lets get the new values for each.
        //To do this, we're just going to take the first number
        //so, if we replaced 5 by 77, we're taking the first 7 and we are done.
        //all the 5 are now a 7, (as an example).

        for (int x = 0; x < inputMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < inputMatrix.GetLength(1); y++)
            {
                inputMatrix[x, y] = int.Parse(inputMatrix[x, y].ToString()[..1]);
            }
        }

        return inputMatrix;
    }

    private async Task<int> FindNewNumberAsync(List<BoardRandomizer> boardRandomizer, int currentNumber)
    {
        return await Task.Run(() =>
        {
            return boardRandomizer.First(a => a.Number.Equals(currentNumber)).ReplacedNumber;
        }).ConfigureAwait(false);
    }

    private async Task<List<BoardRandomizer>> GetBoardRandomizersAsync()
    {
        List<BoardRandomizer> BoardRandomizers = new List<BoardRandomizer>();

        var IntegerList = await GetRandomizedIntegerListAsync().ConfigureAwait(false);
        int j = 1;
        foreach (int item in IntegerList)
        {
            var NewNumber = item;
            //convert 1 in 11, 2 in 22 and so on ...
            for (int i = 0; i < 10; i++)
            {
                NewNumber += item;
            }
            BoardRandomizers.Add(new BoardRandomizer { Number = j, ReplacedNumber = NewNumber });
            j++;
        }

        return BoardRandomizers;
    }

    /// <summary>
    /// Saves a matrix into a new file
    /// </summary>
    /// <param name="matrix"></param>
    public async Task SaveAsync(int[,] matrix)
    {
        StringBuilder sb = new StringBuilder();
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                sb.Append(matrix[x, y]);
            }
            sb.AppendLine();
        }
        if (!Directory.Exists("sudoku/"))
        {
            Directory.CreateDirectory("sudoku/");
        }

        await File.WriteAllTextAsync(@"sudoku/" + Guid.NewGuid() + ".txt", sb.ToString()).ConfigureAwait(false);
    }

    /// <summary>
    /// Given a matrix return a valid board with removed values ready to be played.
    /// </summary>
    /// <param name="difficulty">1 Easy, 2 Medim</param>
    /// <returns></returns>
    public async Task<int[,]> PrepareBoardAsync(Difficulty difficulty, int[,] matrix)
    {
        int ToDelete = 0;
        switch (difficulty)
        {
            case Difficulty.Easy:
                ToDelete = 24; //57 remain
                break;
            case Difficulty.Medium:
                ToDelete = 44; // 37 remain
                break;
            case Difficulty.Hard:
                ToDelete = 54; //27 remain
                break;
            default:
                break;
        }

        //Iterate the amount of numbers I'm going to delete.
        int Deleted = 0;
        while (Deleted < (ToDelete - 1))
        {
            //Go through all 9 quadrants, from 1 to 9
            for (int QuadrantImWorkingOn = 1; QuadrantImWorkingOn <= 9; QuadrantImWorkingOn++)
            {
                if (Deleted == (ToDelete - 1)) { break; }
                await new SudokuValidations().DeleteRandomNumberOfThisQuadrantAsync(matrix, QuadrantImWorkingOn).ConfigureAwait(false);
                Deleted++;
            }
        }

        return matrix;
    }
}