using System;
using SudokuWebMVC.Helpers;

namespace SudokuUnitTest;

public class _2DTests
{
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(2, 1, 1)]
    [InlineData(3, 1, 1)]
    [InlineData(4, 1, 1)]
    [InlineData(1, 2, 1)]
    [InlineData(2, 2, 1)]
    [InlineData(3, 2, 1)]
    [InlineData(4, 2, 1)]
    [InlineData(1, 3, 1)]
    [InlineData(2, 3, 1)]
    [InlineData(3, 3, 1)]
    [InlineData(4, 3, 1)]
    [InlineData(1, 4, 1)]
    [InlineData(2, 4, 1)]
    [InlineData(3, 4, 1)]
    [InlineData(4, 4, 1)]
    public void LongRunningTest(int threads, int method, int hours)
    {
        LongRunMethod(threads, method, hours);
    }

    private async void LongRunMethod(int threads, int method, int hours)
    {
        Sudoku sudoku = new Sudoku();

        using var cts = new CancellationTokenSource(TimeSpan.FromHours(hours));
        var token = cts.Token;

        List<Task> tasks = new List<Task>();
        // You can increase the number of tasks if needed
        for (int i = 0; i < threads; i++)
        {
            tasks.Add(Task.Run(() => sudoku.GenerateRandom(method, threads, token)));
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            // Test ran for one hour and was cancelled as expected
        }
    }
}
