using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SudokuWebMVC.Services;

namespace SudokuWebMVC.Validations;

public class SudokuValidations
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="array">Multidimensional array to validate</param>
    /// <param name="complete">True when validating all the board, False when validating inner matrix</param>
    /// <returns></returns>
    public async Task<bool> ValidateBySumAsync(int[,] array, bool complete = true)
    {
        return await Task.Run(() =>
        {
            int sumResult = 0;
            foreach (var value in array)
            {
                sumResult += value;
            }

            return sumResult == (complete ? 405 : 45);
        }).ConfigureAwait(false);
    }


    /// <summary>
    /// single validation by sum is performed first. Sum of numbers from 1 to 9 is equal to 45
    /// </summary>
    /// <param name="singleArray"></param>
    /// <returns></returns>
    public async Task<bool> ValidateRowOrColumnAsync(int[] singleArray)
    {
        //before proceding to do a full validation, we need to validate the sum of the array to be 45
        return singleArray.Sum(a => a) == 45 ? await ValidateDuplicatedNumbersInArrayAsync(singleArray).ConfigureAwait(false) : false;
    }

    public async Task<bool> ValidateAllInnerMatrixAsync(int[,] matrix)
    {
        return await Task.Run(async () =>
        {
            int x = 0;
            int y = 0;

            for (int i = 0; i < 9; i++)
            {
                if (i == 3 || i == 6 || i == 9)
                {
                    y = 0;
                    x += 3;
                }

                var smallMatrix = await MatrixToSmallMatrixAsync(matrix, x, x + 2, y, y + 2).ConfigureAwait(false);
                var IsInnerMatrixValid = await ValidateInnerMatrixAsync(smallMatrix).ConfigureAwait(false);

                if (!IsInnerMatrixValid) return false;
                y += 3;
            }

            return true;
        }).ConfigureAwait(false);

    }

    public async Task<bool> ValidateInnerMatrixAsync(int[,] innerMatrix)
    {
        //Sum of the inner matrix should be 45
        if (await ValidateBySumAsync(innerMatrix, false).ConfigureAwait(false))
        {
            //Convert to unidimensional array.
            var singleArray = await MatrixToSingleArrayAsync(innerMatrix).ConfigureAwait(false);
            return await ValidateDuplicatedNumbersInArrayAsync(singleArray).ConfigureAwait(false);
        }
        else return false;
    }

    public async Task<bool> ValidateRowsAndColumnsAsync(int[,] matrix)
    {
        var rowTasks = new List<Task<bool>>();
        var columnTasks = new List<Task<bool>>();

        // Validate all rows
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            rowTasks.Add(ValidateRowOrColumnAsync(await GetRowAsync(matrix, x)));
        }

        // Validate all columns
        for (int y = 0; y < matrix.GetLength(1); y++)
        {
            columnTasks.Add(ValidateRowOrColumnAsync(await GetColumnAsync(matrix, y)));
        }

        var rowResults = await Task.WhenAll(rowTasks);
        var columnResults = await Task.WhenAll(columnTasks);

        return rowResults.All(result => result) && columnResults.All(result => result);
    }

    /// <summary>
    /// Convert a matrix to a unidimensional array from left to right, from top to bottom.
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public async Task<int[]> MatrixToSingleArrayAsync(int[,] matrix)
    {
        return await Task.Run(() =>
        {
            int[] singleArray = new int[matrix.GetLength(0) * matrix.GetLength(1)];
            int i = 0;
            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    singleArray[i] = matrix[x, y];
                    i++;
                }
            }

            //TODO
            //Buffer.BlockCopy()

            return singleArray;
        }).ConfigureAwait(false);
    }

    public async Task<int[,]> MatrixToSmallMatrixAsync(int[,] matrix, int startX, int endX, int startY, int endY)
    {
        return await Task.Run(() =>
        {
            int[,] InnerMatrix = new int[3, 3];
            int NewX = 0;
            int NewY = 0;
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    InnerMatrix[NewX, NewY] = matrix[x, y];
                    NewY++;
                }
                NewX++;
                NewY = 0;
            }

            return InnerMatrix;
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Validate there are no duplicated numbers inside unidimensional array
    /// </summary>
    /// <param name="array"></param>
    /// <returns>True when validation succeed</returns>
    public async Task<bool> ValidateDuplicatedNumbersInArrayAsync(int[] array)
    {
        return await Task.Run(() =>
        {
            var result = array.GroupBy(x => x).Select(x => new { key = x.Key, val = x.Count() });
            return !result.Any(a => a.val > 1);
        }).ConfigureAwait(false);
    }

    public async Task<int[]> GetColumnAsync(int[,] matrix, int columnNumber)
    {
        return await Task.Run(() =>
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }).ConfigureAwait(false);
    }


    public async Task<int[]> GetRowAsync(int[,] matrix, int rowNumber)
    {
        return await Task.Run(() =>
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
        }).ConfigureAwait(false);
    }

    public async Task<bool> ValidateDuplicatedTemporalValuesInArrayAsync(int[] array)
    {
        return await Task.Run(() =>
        {
            var result = array.GroupBy(x => x).Select(x => new { key = x.Key, val = x.Count() });
            return !result.Any(a => a.key != 0 && a.val > 1);
        }).ConfigureAwait(false);
    }

    /// Deletes a random number from a specified quadrant in the given Sudoku matrix.
    /// </summary>
    /// <param name="matrix">The Sudoku matrix represented as a 2D integer array.</param>
    /// <param name="quadrant">The quadrant from which a number will be deleted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// This method will recursively attempt to delete a number from the specified quadrant until a non-zero number is found and deleted.
    /// </remarks>
    public async Task DeleteRandomNumberOfThisQuadrantAsync(int[,] matrix, int quadrant)
    {
        var SudokuOrderForRemoving = new SudokuOrderForAdding().GetSudokuOrderForAdding_OrderedMethod();
        var Coordinates = SudokuOrderForRemoving.Where(a => a.Value.Equals(quadrant)).First();

        //The following +1 is necessary to include the highest coordinate value.
        int RandomX = new Random().Next(Coordinates.XCoordinate, Coordinates.XCoordinate + 2 + 1);
        int RandomY = new Random().Next(Coordinates.YCoordinate, Coordinates.YCoordinate + 2 + 1);

        //Call this method recursively until we delete a valid number (x!=0)
        if (matrix[RandomX, RandomY].Equals(0))
        {
            await DeleteRandomNumberOfThisQuadrantAsync(matrix, quadrant).ConfigureAwait(false);
        }
        else
        {
            matrix[RandomX, RandomY] = 0;
        }
    }

    /// Asynchronously checks if the given Sudoku matrix is complete and valid.
    /// </summary>
    /// <param name="matrix">The Sudoku matrix to validate.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the matrix is complete and valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided matrix is null.</exception>
    public async Task<bool> MatrixIsDoneAsync(int[,] matrix)
    {
        if (matrix is null)
        {
            throw new ArgumentNullException(nameof(matrix));
        }

        var isValidBySum = await new SudokuValidations().ValidateBySumAsync(matrix).ConfigureAwait(false);
        var isValidByRowsAndColumns = await new SudokuValidations().ValidateRowsAndColumnsAsync(matrix).ConfigureAwait(false);
        var isValidByInnerMatrix = await new SudokuValidations().ValidateAllInnerMatrixAsync(matrix).ConfigureAwait(false);

        return isValidBySum && isValidByRowsAndColumns && isValidByInnerMatrix;
    }
}