using System.Collections.Generic;
using System.Linq;

namespace SudokuWebMVC.Helpers
{
    public class Sudoku3D
    {
        /// <summary>
        /// Function to determine if a 3D board is valid or not, following Sudoku conventions.
        /// </summary>
        /// <param name="board">List of boards represented by a bidimentional array. In order from the front to the back./param>
        /// <returns></returns>
        public bool ValidateEntireCube(List<int[,]> cube)
        {
            // To be easier to understand, we are going to compare this cube with a rubiks cube
            // by only following the rubiks color convention.

            // We assume that the cube is formed from front to back, and that it is ordered,
            // so that element 0 in the list is the front face (blue),
            // and element 8 is the rearmost face of the cube (green)
            // so boards will be the same that cubeFacignBlue

            //1. Validate all 9 boards completely facing front (Blue face towards green face)
            if(!ValidateBoard(cube)) return false;

            //2. Get Cube (Red face towars Orange face) and validate (left to right)
            var cubeFacingRed = ReturnFacingRed(cube);
            if (!ValidateBoard(cubeFacingRed)) return false;

            //3. Get Cube (Yellow face towars white face) and validate (bottom to top)
            var cubeFacingYellow = ReturnFacingYellow(cube);
            if (!ValidateBoard(cubeFacingYellow)) return false;

            //4. Validate from the blue side to the green side (Z-index)
            if (!ValidateLineFromCubeFrontToBack(cube)) return false;

            //5. Validate from the red side to the orange (X-Index)
            if (!ValidateLineFromCubeFrontToBack(cubeFacingRed)) return false;

            //6. Validate from the yellow side to the white (Y-index)
            if (!ValidateLineFromCubeFrontToBack(cubeFacingYellow)) return false;

            //7. Validate each inner cube (27)
            if(!ValidateInnerCubes(cube)) return false;
            
            return true;
        }

        /// <summary>
        /// Given a cube, validate all its layers
        /// </summary>
        /// <param name="cube"></param>
        private bool ValidateBoard(List<int[,]> cube)
        {
            SudokuValidations validator = new SudokuValidations();
            foreach (var board in cube)
            {
                if (!validator.MatrixIsDone(board))
                {
                    return false;
                }
            }

            return true;
        }

        private List<int[,]> ReturnFacingRed(List<int[,]> cube)
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
                        matrix[y, x] = cube[8 - x][z, y];
                    }
                }
                rotatedBoards.Add(matrix);
            }

            return rotatedBoards;
        }

        private List<int[,]> ReturnFacingYellow(List<int[,]> cube)
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
                        matrix[y, x] = cube[8 - x][8 - z, 8 - y];
                    }
                }
                rotatedBoards.Add(matrix);
            }

            return rotatedBoards;
        }

        private bool ValidateLineFromCubeFrontToBack(List<int[,]> cube)
        {
            //get an array facing front to back
            for (int z = 0; z < 9; z++)
            {
                for (int y = 0; y < 9; y++)
                {
                    int[] array = new int[9];
                    for (int x = 0; x < 9; x++)
                    {
                        array[x] = cube[x][z,y];
                    }

                    if (!new SudokuValidations().ValidateRowOrColumn(array)) return false;
                }
            }

            return true;
        }

        private bool ValidateInnerCubes(List<int[,]> cube)
        {
            //variables used to increment the X-Y-Z Axis value, generally by 3, or set to 0 to move the index in order to get the next inner cube
            int X_Add = 0;
            int Y_Add = 0;
            int Z_Add = 0;

            //Array to handle the increment or reset the previous declared variables.
            int[] x_3_list = new int[18] { 1, 2, 4, 5, 7, 8, 10, 11, 13, 14, 16, 17, 19, 20, 22, 23, 25, 26 };
            int[] y_3_list = new int[6] { 3, 6, 12, 15, 21, 24 };
            int[] z_3_list = new int[2] { 9, 18 };
            int[] x_0_list = new int[9] { 3, 6, 9, 12, 15, 18, 21, 24, 27 };
            int[] y_0_list = new int[3] { 9, 18, 27 };

            //We need to get 27 cubes
            for (int i = 1; i <= 27; i++)
            {
                //List to store all the cube in a linear way.
                List<int> innerCube = new List<int>();
                for (int z = 0; z < 3; z++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            innerCube.Add(cube[z + Z_Add][y + Y_Add, x + X_Add]);
                        }
                    }
                }

                //validate the sum of the entire cube
                if (!innerCube.Sum().Equals(135)) { return false; }
                
                //validate all numbers repeat only 3 times.
                if (!innerCube.GroupBy(x => x).All(g => g.Count() == 3 && g.Key >= 1 && g.Key <= 9)) { return false; }

                //manipulate X-Y-Z index according to the number of the inner cube
                if (x_3_list.Where(x => x.Equals(i)).Any())
                {
                    X_Add = X_Add + 3;
                }
                if (x_0_list.Where(x => x.Equals(i)).Any())
                {
                    X_Add = 0;
                }
                if (y_3_list.Where(x => x.Equals(i)).Any())
                {
                    Y_Add = Y_Add + 3;
                }
                if (y_0_list.Where(x => x.Equals(i)).Any())
                {
                    Y_Add = 0;
                }
                if (z_3_list.Where(x => x.Equals(i)).Any())
                {
                    Z_Add = Z_Add + 3;
                }
            }

            return true;
        }

        public void Create3DCube()
        {
            
        }

        //private int[,] Get3x3Cube()
        //{
        //    int[,,] cube = new int[3, 3, 3];
        //    w
        //}

        public bool Validate3x3Cube(int[,,] cube)
        {
            //1. No repeated numbers on each layer            
            for (int z = 0; z < 3; z++)
            {
                List<int> Uniquelist = new List<int>();
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        if (Uniquelist.Exists(a => a.Equals(cube[x, y, z])))
                        {
                            return false;
                        }
                        else
                        {
                            Uniquelist.Add(cube[x, y, z]);
                        }
                    }
                }
            }

            //2. No repeated numbers on each layer

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (cube[y, x, 0] == cube[y, x, 1] || cube[y, x, 0] == cube[y, x, 2])
                    {
                        return false;
                    }

                }
            }

            return true;
        }
    }
}
