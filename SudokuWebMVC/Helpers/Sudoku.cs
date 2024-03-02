using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SudokuWebMVC.Enum;

namespace SudokuWebMVC.Helpers
{
    public class Sudoku
    {

        public void GenerateRandom(int Method)
        {
            int[,] matrix = new int[9, 9];
            bool isValid = false;
            DateTime start = DateTime.UtcNow;

            if (Method.Equals(5))
            {
                new SudokuGenerator().RandomizeFromFolder();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Process Finished");
            }
            else
            {
                while (true)
                {
                    matrix = new SudokuGenerator().LoadRandom(Method);

                    isValid = new SudokuValidations().MatrixIsDone(matrix);

                    if (isValid)
                    {
                        var end = DateTime.UtcNow;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Found in {(end.Subtract(start).TotalMinutes)} minutes");
                        PrintMatrix(matrix);
                        Console.ForegroundColor = ConsoleColor.White;
                        new SudokuGenerator().Save(matrix);
                        start = DateTime.UtcNow;
                    }
                }
            }
        }

        public void PrintMatrix(int[,] matrix)
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
        }
    }

    public class SudokuGenerator
    {
        public class BoardRandomizer
        {
            public int Number { get; set; }
            public int ReplacedNumber { get; set; }
        }
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
            int MaxAttempts = (9 * 8 * 7 * 6 * 5 * 4 * 3 * 2 * 1);
            int CurrentLastAttempt = 1;
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
                    FinalMatrix = FinalMatrixCopy;
                    //this group has passed the validations.
                    sudokuOrderForAdding[CurrentOrder.Order - 1].Done = true;
                    Console.WriteLine("Group Added " + CurrentOrder.Value + " after " + CurrentLastAttempt + " attempts");
                    CurrentLastAttempt = 1;
                }

                CurrentLastAttempt++;
            }

            return FinalMatrix;
        }

        /// <summary>
        /// Loads a valid board from file
        /// </summary>
        /// <returns></returns>
        public int[,] LoadFromFile()
        {
            string Path = @"/PathToFolder/";

            try
            {

                var Files = Directory.GetFiles(Path);
                int Random = new Random().Next(0, Files.Count());
                return new SudokuValidator().ConvertFileToMatrix(File.ReadAllLines(Files[Random]));

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

        //add matrix piece to another matrix given a specified coordenates of the location.
        // i.e., add section 3 from a matrix A to section 3 of matrix B, being 3 the third upper right quadrant.
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

        /// <summary>
        /// Gets a 3x3 matrix with random data
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets an integer list with 9 random rumbers.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Given a folder with valid sudoku's files, randomizes the content of each one to create 
        /// more valid boards. This will run forever until you stop it or blows your computer up.
        /// </summary>
        public void RandomizeFromFolder()
        {
            string FolderPath = @"sudoku/";
            Console.ForegroundColor = ConsoleColor.Green;

            while (true)
            {
                foreach (string item in Directory.GetFiles(FolderPath))
                {
                    //get current board
                    var Matrix = new SudokuValidator().ConvertFileToMatrix(File.ReadAllLines(item));
                    //validate first this is a valid board
                    if (new SudokuValidations().MatrixIsDone(Matrix))
                    {
                        //replace by new values
                        var NewMatrix = ReplaceMatrix(Matrix);
                        if (new SudokuValidations().MatrixIsDone(NewMatrix))
                        {
                            Save(NewMatrix);
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
        private int[,] ReplaceMatrix(int[,] inputMatrix)
        {
            //find and replace all current values for new ones,
            //For example, all #1 by 33, all #2 by 77, all #3 by 99 and so on.
            //Those are random generated numbers.

            var BoardRandomizer = GetBoardRandomizers();
            for (int x = 0; x < inputMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < inputMatrix.GetLength(1); y++)
                {
                    inputMatrix[x, y] = FindNewNumber(BoardRandomizer, inputMatrix[x, y]);
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

        private int FindNewNumber(List<BoardRandomizer> boardRandomizer, int currentNumber)
        {
            return boardRandomizer.Where(a => a.Number.Equals(currentNumber)).First().ReplacedNumber;
        }

        private List<BoardRandomizer> GetBoardRandomizers()
        {
            List<BoardRandomizer> BoardRandomizers = new List<BoardRandomizer>();

            var IntegerList = GetRandomizedIntegerList();
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
        public void Save(int[,] matrix)
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
            File.WriteAllText(@"sudoku/" + Guid.NewGuid() + ".txt", sb.ToString());
        }

        /// <summary>
        /// Given a matrix return a valid board with removed values ready to be played.
        /// </summary>
        /// <param name="difficulty">1 Easy, 2 Medim</param>
        /// <returns></returns>
        public int[,] PrepareBoard(Difficulty difficulty, int[,] matrix)
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
                    new SudokuValidations().DeleteRandomNumberOfThisQuadrant(ref matrix, QuadrantImWorkingOn);
                    Deleted++;
                }
            }

            return matrix;
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

        /// <summary>
        /// single validation by sum is performed first. Sum of numbers from 1 to 9 is equal to 45
        /// </summary>
        /// <param name="singleArray"></param>
        /// <returns></returns>
        public bool ValidateRowOrColumn(int[] singleArray)
        {
            if (singleArray.Sum(a => a) == 45)
            {
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
                if (i == 3 || i == 6 || i == 9) { y = 0; x += 3; }
                if (!ValidateInnerMatrix(MatrixToSmallMatrix(matrix, x, x + 2, y, y + 2))) return false;
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
                return ValidateDuplicatedNumbersInArray(MatrixToSingleArray(innerMatrix));
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

        public void DeleteRandomNumberOfThisQuadrant(ref int[,] matrix, int quadrant)
        {
            var SudokuOrderForRemoving = new SudokuOrderForAdding().GetSudokuOrderForAdding_OrderedMethod();
            var Coordinates = SudokuOrderForRemoving.Where(a => a.Value.Equals(quadrant)).First();
            
            //The following +1 is necessary to include the highest coordinate value.
            int RandomX = new Random().Next(Coordinates.XCoordinate, (Coordinates.XCoordinate + 2) + 1);
            int RandomY = new Random().Next(Coordinates.YCoordinate, (Coordinates.YCoordinate + 2) + 1);

            //Call this method recursively until we delete a valid number (x!=0)
            if (matrix[RandomX, RandomY].Equals(0))
            {
                DeleteRandomNumberOfThisQuadrant(ref matrix, quadrant);
            }
            else
            {
                matrix[RandomX, RandomY] = 0;
            }
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
            if (files.Count() == 0)
            {
                Console.WriteLine($"No Files found for given path: {path}");
                return;
            }
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

        public int[,] ConvertFileToMatrix(string[] content)
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
