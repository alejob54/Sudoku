using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SudokuWebMVC.Helpers
{
    public class Sudoku
    {

        public void GenerateRandom(int Method)
        {
            int[,] matrix = new int[9, 9];
            bool isValid = false;
            DateTime start = DateTime.UtcNow;

            while (true)
            {
                matrix = new SudokuGenerator().LoadRandom(Method);
                isValid = new SudokuValidations().MatrixIsDone(matrix);

                if (isValid)
                {
                    var end = DateTime.UtcNow;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Found in " + (end.Subtract(start).TotalMinutes));
                    PrintMatrix(matrix);
                    Console.ForegroundColor = ConsoleColor.White;
                    Save(matrix);
                    start = DateTime.UtcNow;
                }
            }
        }

        private void Save(int[,] matrix)
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
            if (!Directory.Exists(@"C:/sudoku/"))
            {
                Directory.CreateDirectory(@"C:/sudoku/");
            }
            File.WriteAllText(@"C:/sudoku/" + Guid.NewGuid() + ".txt", sb.ToString());
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

    public class SudokuGenerator
    {
        public int[,] LoadRandom(int Method)
        {
            var FinalMatrix = new int[9, 9];
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
            int MaxAttempts = (9*8*7*6*5*4*3*2*1);
            int CurrentLastAttempt = 0;
            while (!new SudokuValidations().MatrixIsDone(FinalMatrix))
            {
                if (CurrentLastAttempt > MaxAttempts)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Reaching limit of combinations --- > Skipping");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                }
                var CurrentOrder = sudokuOrderForAdding.Where(a => !a.Done).Take(1).First();
                var TemporalMatrix = GetTentativeValues();
                var FinalMatrixCopy = FinalMatrix;

                AddToMatrix(ref FinalMatrixCopy, TemporalMatrix, CurrentOrder);

                //Validate. 
                if (ValidateTemporalAdding(FinalMatrixCopy))
                {
                    CurrentLastAttempt = 0;
                    FinalMatrix = FinalMatrixCopy;
                    //this group has passed the validations.
                    sudokuOrderForAdding[CurrentOrder.Order - 1].Done = true;
                    Console.WriteLine("Group Added " + CurrentOrder.Value);
                }
                
                CurrentLastAttempt++;
            }

            return FinalMatrix;
        }

        public bool ValidateTemporalAdding(int[,] matrix)
        {
            var Validations = new SudokuValidations();

            //rows
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var array = Validations.GetRow(matrix, i);
                if (!Validations.ValidateDuplicatedTemporalValuesInArray(array))
                {
                    return false;
                }
            }

            //columns
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                var array = Validations.GetColumn(matrix, i);
                if (!Validations.ValidateDuplicatedTemporalValuesInArray(array))
                {
                    return false;
                }
            }

            return true;
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

    }

    public class SudokuValidations
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array">Multidimensional array to validate</param>
        /// <param name="complete">True when validating all the board, False when validating inner matrix</param>
        /// <returns></returns>
        public bool ValidateBySum(int[,] array, bool complete = true)
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

        public bool ValidateRowOrColumn(int[] singleArray)
        {
            //single validation by sum is performed first. Sum of numbers from 1 to 9 is equal to 45
            if (singleArray.Sum(a => a) == 45)
            {
                //Additional Validation. Find duplicate numbers
                return ValidateDuplicatedNumbersInArray(singleArray);
            }
            else return false;
        }

        public bool ValidateAllInnerMatrix(int[,] matrix)
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
                var matx = MatrixToSmallMatrix(matrix, x, x + 2, y, y + 2);
                if (!ValidateInnerMatrix(matx)) return false;
                y += 3;

            }

            return true;
        }

        public bool ValidateInnerMatrix(int[,] innerMatrix)
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

        public bool ValidateRowsAndColumns(int[,] matrix)
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

        /// <summary>
        /// Convert a matrix to a unidimensional array from left to right, from top to bottom.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public int[] MatrixToSingleArray(int[,] matrix)
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

        public int[,] MatrixToSmallMatrix(int[,] matrix, int startX, int endX, int startY, int endY)
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
        public bool ValidateDuplicatedNumbersInArray(int[] array)
        {
            var result = array.GroupBy(x => x).Select(x => new { key = x.Key, val = x.Count() });
            return !result.Where(a => a.val > 1).Any();
        }

        public int[] GetColumn(int[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public int[] GetRow(int[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }

        public bool ValidateDuplicatedTemporalValuesInArray(int[] array)
        {
            var result = array.GroupBy(x => x).Select(x => new { key = x.Key, val = x.Count() });
            return !result.Where(a => a.key != 0 && a.val > 1).Any();
        }

        public bool MatrixIsDone(int[,] matrix)
        {
            if (matrix is null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            if (new SudokuValidations().ValidateBySum(matrix) &&
                new SudokuValidations().ValidateRowsAndColumns(matrix) &&
                new SudokuValidations().ValidateAllInnerMatrix(matrix))
            {
                return true;
            }
            else return false;
        }

    }

    public class SudokuValidator
    {
        public void ValidateFolder(string path, bool deleteInvalidFiles = false)
        {
            var files = Directory.GetFiles(path);
            int i = 1;
            foreach (var item in files)
            {
                int[,] Matrix = ConvertFileToMatrix(File.ReadAllLines(item));
                bool IsValid = new SudokuValidations().MatrixIsDone(Matrix);
                if (IsValid)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{i} - {item} is valid");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{i} - {item} is invalid");

                    if (deleteInvalidFiles)
                    {
                        File.Delete(item);
                    }
                }
                i++;
            }
        }

        private int[,] ConvertFileToMatrix(string[] content)
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
        public List<SudokuOrderForAdding> GetSudokuOrderForAdding_CrossMethod()
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

        public List<SudokuOrderForAdding> GetSudokuOrderForAdding_OrderedMethod()
        {
            return new List<SudokuOrderForAdding>
            {
                new SudokuOrderForAdding { Order = 1, Value = 1, XCoordinate = 0, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 2, Value = 2, XCoordinate = 0, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 3, Value = 3, XCoordinate = 0, YCoordinate = 6 },
                new SudokuOrderForAdding { Order = 4, Value = 4, XCoordinate = 3, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 5, Value = 5, XCoordinate = 3, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 6, Value = 6, XCoordinate = 3, YCoordinate = 6 },
                new SudokuOrderForAdding { Order = 7, Value = 7, XCoordinate = 6, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 8, Value = 8, XCoordinate = 6, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 9, Value = 9, XCoordinate = 6, YCoordinate = 6 }
            };
        }
        public List<SudokuOrderForAdding> GetSudokuOrderForAdding_SpiralMethod()
        {
            return new List<SudokuOrderForAdding>
            {
                new SudokuOrderForAdding { Order = 1, Value = 1, XCoordinate = 0, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 2, Value = 2, XCoordinate = 0, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 3, Value = 3, XCoordinate = 0, YCoordinate = 6 },
                new SudokuOrderForAdding { Order = 4, Value = 6, XCoordinate = 3, YCoordinate = 6 },
                new SudokuOrderForAdding { Order = 5, Value = 9, XCoordinate = 6, YCoordinate = 6 },
                new SudokuOrderForAdding { Order = 6, Value = 8, XCoordinate = 6, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 7, Value = 7, XCoordinate = 6, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 8, Value = 4, XCoordinate = 3, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 9, Value = 5, XCoordinate = 3, YCoordinate = 3 }
            };
        }

        public List<SudokuOrderForAdding> GetSudokuOrderForAdding_SpiralInvertedMethod()
        {
            return new List<SudokuOrderForAdding>
            {
                new SudokuOrderForAdding { Order = 1, Value = 5, XCoordinate = 3, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 2, Value = 6, XCoordinate = 3, YCoordinate = 6 },
                new SudokuOrderForAdding { Order = 3, Value = 3, XCoordinate = 0, YCoordinate = 6 },
                new SudokuOrderForAdding { Order = 4, Value = 2, XCoordinate = 0, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 5, Value = 1, XCoordinate = 0, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 6, Value = 4, XCoordinate = 3, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 7, Value = 7, XCoordinate = 6, YCoordinate = 0 },
                new SudokuOrderForAdding { Order = 8, Value = 8, XCoordinate = 6, YCoordinate = 3 },
                new SudokuOrderForAdding { Order = 9, Value = 9, XCoordinate = 6, YCoordinate = 6 }
            };
        }

    }
}
