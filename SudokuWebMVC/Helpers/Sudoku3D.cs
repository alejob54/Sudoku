using System.Collections.Generic;

namespace SudokuWebMVC.Helpers
{
    public class Sudoku3D
    {
        /// <summary>
        /// Function to determine if a 3D board is valid or not, following Sudoku conventions.
        /// </summary>
        /// <param name="board">List of boards represented by a bidimentional array. In order from the front to the back./param>
        /// <returns></returns>
        public bool ValidateEntireCube(List<int[,]> boards)
        {
            // To be easier to understand, we are going to compare this cube with a rubiks cube
            // by only following the rubiks color convention.

            // We assume that the cube is formed from front to back, and that it is ordered,
            // so that element 0 in the list is the front face (blue),
            // and element 8 is the rearmost face of the cube (green)

            //1. Validate all 9 boards completely facing front (Blue face towards green face)
            if(!ValidateBoard(boards)) return false;

            //2. Get Cube (Red face towars Orange face) and validate (left to right)
            if (!ValidateBoard(ReturnFacingRed(boards))) return false;

            //3. Get Cube (Yellow face towars white face) and validate (bottom to top)
            if (!ValidateBoard(ReturnFacingYellow(boards))) return false;

            //4. Validate from the blue side to the green side (Z-index)

            //5. Validate from the red side to the orange (X-Index)

            //6. Validate from the yellow side to the white (Y-index)

            //7. Validate each inner cube (27)

            return true;
        }

        /// <summary>
        /// Given a cube, validate all its layers
        /// </summary>
        /// <param name="boards"></param>
        private bool ValidateBoard(List<int[,]> boards)
        {
            SudokuValidations validator = new SudokuValidations();
            foreach (var board in boards)
            {
                if (!validator.MatrixIsDone(board))
                {
                    return false;
                }
            }

            return true;
        }

        private List<int[,]> ReturnFacingRed(List<int[,]> boards)
        {
            List<int[,]> rotatedBoards = new List<int[,]>();

            // This loop handles the loop facing-front and going to the rear (back) Z-index
            for (int z = 0; z < 9; z++)
            {
                var matrix = new int[9, 9];
                // This loop handles the Y-index (from top to botom)
                for (int y = 0; y < 9; y++)
                {
                    // This loop handles the X-index (from left to right)
                    for (int x = 0; x < 9; x++)
                    {
                        matrix[y, x] = boards[8 - x][z, y];
                    }
                }
                rotatedBoards.Add(matrix);
            }

            return rotatedBoards;
        }

        private List<int[,]> ReturnFacingYellow(List<int[,]> boards)
        {
            List<int[,]> rotatedBoards = new List<int[,]>();

            // This loop handles the loop facing-front and going to the rear (back) Z-index
            for (int z = 0; z < 9; z++)
            {
                var matrix = new int[9, 9];
                // This loop handles the Y-index (from bottom to top)
                for (int y = 0; y < 9; y++)
                {
                    // This loop handles the X-index (from left to right)
                    for (int x = 0; x < 9; x++)
                    {
                        matrix[y, x] = boards[8 - x][8 - z, 8 - y];
                    }
                }
                rotatedBoards.Add(matrix);
            }

            return rotatedBoards;
        }
    }
}
