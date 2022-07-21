using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuWebMVC.Helpers
{
    public class Sudoku
    {

        public int[,] Generate()
        {
            var matrix = LoadValidBoard();
            return Validate(matrix) ? matrix : default;
        }

        public int[,] GenerateRandom()
        {
            int[,] matrix = new int[9, 9];
            bool isValid = false;

            while (!isValid)
            {
                matrix = LoadRandom();
                Console.WriteLine("Validating");
                PrintMatrix(matrix);
                isValid = Validate(matrix);
            }

            return matrix;
        }

        private int[,] LoadRandom()
        {
            var FinalMatrix = new int[9, 9];
            var sudokuOrderForAdding = new SudokuOrderForAdding().GetSudokuOrderForAddings();

            while (!MatrixIsDone(FinalMatrix))
            {
                var CurrentOrder = sudokuOrderForAdding.Where(a => !a.Done).Take(1).First();
                var TemporalMatrix = GetTentativeValues();
                var FinalMatrixCopy = FinalMatrix;

                AddToMatrix(ref FinalMatrixCopy, TemporalMatrix, CurrentOrder);

                //Validate. 
                if (ValidateTemporalAdding(FinalMatrixCopy))
                {
                    FinalMatrix = FinalMatrixCopy;
                    //this group has passed the validations.
                    sudokuOrderForAdding[CurrentOrder.Order - 1].Done = true;
                    Console.WriteLine("Group Added " + CurrentOrder.Order);
                }
            }

            return FinalMatrix;
        }

        private bool ValidateTemporalAdding(int[,] matrix)
        {
            //rows
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var array = GetRow(matrix, i);
                if (!ValidateDuplicatedTemporalValuesInArray(array))
                {
                    return false;
                }
            }

            //columns
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                var array = GetColumn(matrix, i);
                if (!ValidateDuplicatedTemporalValuesInArray(array))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateDuplicatedTemporalValuesInArray(int[] array)
        {
            var result = array.GroupBy(x => x).Select(x => new { key = x.Key, val = x.Count() });
            return !result.Where(a => a.key != 0 && a.val > 1).Any();
        }

        private void AddToMatrix(ref int[,] destination, int[,] origin, SudokuOrderForAdding order)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    destination[order.XCoordinate + x, order.YCoordinate + y] = origin[x, y];
                }
            }
        }

        private int[,] GetTentativeValues()
        {
            var TmpMatrix = new int[3, 3];
            var Ints = GetRandomizedIntegerList();
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

        private List<int> GetRandomizedIntegerList()
        {
            List<int> ints = new List<int>();
            while (ints.Count < 9)
            {
                Random random = new Random();
                int rnd = random.Next(1, 10);
                if (!ints.Where(a => a.Equals(rnd)).Any())
                {
                    ints.Add(rnd);
                }
            }

            return ints;
        }

        private bool MatrixIsDone(int[,] matrix)
        {
            return ValidateBySum(matrix);
        }

        public bool Validate(int[,] matrix)
        {
            if (matrix is null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            // set all validations in single line after testing.
            if (ValidateBySum(matrix))
            {
                //validate all rows and columns
                if (ValidateRowsAndColumns(matrix))
                {
                    if (ValidateAllInnerMatrix(matrix))
                    {
                        return true;
                    }
                    else return false;
                }
                else return false;

            }
            else return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array">Multidimensional array to validate</param>
        /// <param name="complete">True when validating all the board, False when validating inner matrix</param>
        /// <returns></returns>
        private bool ValidateBySum(int[,] array, bool complete = true)
        {
            int SumResult = 0;
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    SumResult += array[x, y];
                }
            }

            return SumResult == (complete ? 405 : 45);
        }

        private bool ValidateRowOrColumn(int[] singleArray)
        {
            //single validation by sum is performed first. Sum of numbers from 1 to 9 is equal to 45
            if (singleArray.Sum(a => a) == 45)
            {
                //Additional Validation. Find duplicate numbers
                return ValidateDuplicatedNumbersInArray(singleArray);
            }
            else return false;
        }

        private bool ValidateAllInnerMatrix(int[,] matrix)
        {
            int x = 0;
            int y = 0;

            for (int i = 0; i < 9; i++)
            {
                if (i == 3 || i == 6 || i == 9)
                {
                    y = 0;
                    x = x + 3;
                }
                var matx = MatrixToSmallMatrix(matrix, x, x + 2, y, y + 2);
                if (!ValidateInnerMatrix(matx)) return false;
                y = y + 3;

            }

            return true;
        }

        private bool ValidateInnerMatrix(int[,] innerMatrix)
        {
            //Sum of the inner matrix should be 45
            if (ValidateBySum(innerMatrix, false))
            {
                //Convert to unidimensional array.
                int[] singleArray = MatrixToSingleArray(innerMatrix);
                return ValidateDuplicatedNumbersInArray(singleArray);
            }
            else return false;
        }

        private bool ValidateRowsAndColumns(int[,] matrix)
        {
            //validate all rows
            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                if (!ValidateRowOrColumn(GetRow(matrix, x)))
                {
                    return false;
                }
            }

            //validate all columns
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                if (!ValidateRowOrColumn(GetColumn(matrix, y)))
                {
                    return false;
                }
            }

            return true;
        }
        public SudokuGrid ShowHint(int?[,] array)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            List<SudokuGrid> EmptySquares = new List<SudokuGrid>();
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    if (array[x, y] == null)
                    {
                        EmptySquares.Add(new SudokuGrid { XCoordinate = x, YCoordinate = y });
                    }
                }
            }

            Random random = new Random();
            int RandomPosition = random.Next(1, EmptySquares.Count);
            //Pending to store and compare Sudoku, to be able to find the right number in given position

            return EmptySquares[RandomPosition];
        }

        private int[,] LoadValidBoard()
        {

            var result = new int[9, 9];
            result[0, 0] = 5;
            result[0, 1] = 8;
            result[0, 2] = 3;
            result[0, 3] = 1;
            result[0, 4] = 6;
            result[0, 5] = 4;
            result[0, 6] = 9;
            result[0, 7] = 7;
            result[0, 8] = 2;

            result[1, 0] = 7;
            result[1, 1] = 2;
            result[1, 2] = 9;
            result[1, 3] = 3;
            result[1, 4] = 5;
            result[1, 5] = 8;
            result[1, 6] = 1;
            result[1, 7] = 4;
            result[1, 8] = 6;

            result[2, 0] = 1;
            result[2, 1] = 4;
            result[2, 2] = 6;
            result[2, 3] = 2;
            result[2, 4] = 7;
            result[2, 5] = 9;
            result[2, 6] = 3;
            result[2, 7] = 8;
            result[2, 8] = 5;

            result[3, 0] = 8;
            result[3, 1] = 5;
            result[3, 2] = 2;
            result[3, 3] = 6;
            result[3, 4] = 9;
            result[3, 5] = 1;
            result[3, 6] = 4;
            result[3, 7] = 3;
            result[3, 8] = 7;

            result[4, 0] = 3;
            result[4, 1] = 1;
            result[4, 2] = 7;
            result[4, 3] = 4;
            result[4, 4] = 2;
            result[4, 5] = 5;
            result[4, 6] = 8;
            result[4, 7] = 6;
            result[4, 8] = 9;

            result[5, 0] = 6;
            result[5, 1] = 9;
            result[5, 2] = 4;
            result[5, 3] = 8;
            result[5, 4] = 3;
            result[5, 5] = 7;
            result[5, 6] = 2;
            result[5, 7] = 5;
            result[5, 8] = 1;

            result[6, 0] = 4;
            result[6, 1] = 6;
            result[6, 2] = 5;
            result[6, 3] = 9;
            result[6, 4] = 1;
            result[6, 5] = 3;
            result[6, 6] = 7;
            result[6, 7] = 2;
            result[6, 8] = 8;

            result[7, 0] = 2;
            result[7, 1] = 3;
            result[7, 2] = 1;
            result[7, 3] = 7;
            result[7, 4] = 8;
            result[7, 5] = 6;
            result[7, 6] = 5;
            result[7, 7] = 9;
            result[7, 8] = 4;

            result[8, 0] = 9;
            result[8, 1] = 7;
            result[8, 2] = 8;
            result[8, 3] = 5;
            result[8, 4] = 4;
            result[8, 5] = 2;
            result[8, 6] = 6;
            result[8, 7] = 1;
            result[8, 8] = 3;

            return result;
        }

        /// <summary>
        /// Convert a matrix to a unidimensional array from left to right, from top to bottom.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private int[] MatrixToSingleArray(int[,] matrix)
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

            return singleArray;
        }

        private int[,] MatrixToSmallMatrix(int[,] matrix, int startX, int endX, int startY, int endY)
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
        }

        /// <summary>
        /// Validate there are no duplicated numbers inside unidimensional array
        /// </summary>
        /// <param name="array"></param>
        /// <returns>True when validation succeed</returns>
        private bool ValidateDuplicatedNumbersInArray(int[] array)
        {
            var result = array.GroupBy(x => x).Select(x => new { key = x.Key, val = x.Count() });
            return !result.Where(a => a.val > 1).Any();
        }

        private int[] GetColumn(int[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        private int[] GetRow(int[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }

        public void PrintMatrix(int[,] matrix)
        {
            for (int x = 0; x < matrix.GetLongLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLongLength(1); y++)
                {
                    Console.Write(matrix[x, y]);
                }
                Console.WriteLine("");
            }
        }
    }

    public class SudokuGrid
    {
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        public int Value { get; set; }
    }

    public class SudokuOrderForAdding : SudokuGrid
    {
        public int Order { get; set; }
        public bool Done { get; set; }
        public List<SudokuOrderForAdding> GetSudokuOrderForAddings()
        {
            return new List<SudokuOrderForAdding>
            {
                new SudokuOrderForAdding { Order = 1, Value = 4, XCoordinate = 3, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 2, Value = 5, XCoordinate = 3, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 3, Value = 6, XCoordinate = 3, YCoordinate = 6 },
                new SudokuOrderForAdding { Order = 4, Value = 2, XCoordinate = 0, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 5, Value = 8, XCoordinate = 6, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 6, Value = 1, XCoordinate = 0, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 7, Value = 3, XCoordinate = 0, YCoordinate = 6 },
                new SudokuOrderForAdding { Order = 8, Value = 7, XCoordinate = 6, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 9, Value = 9, XCoordinate = 6, YCoordinate = 6 }
            };
        }
    }
}
