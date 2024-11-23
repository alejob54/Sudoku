using System;
using System.Collections.Generic;
using SudokuWebMVC.Models;

namespace SudokuWebMVC.Services;

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