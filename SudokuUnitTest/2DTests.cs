using SudokuWebMVC.Helpers;

namespace SudokuUnitTest;

public class _2DTests
{
    [Theory]
    [InlineData(1, 1, 30)]
    [InlineData(2, 1, 30)]
    [InlineData(3, 1, 30)]
    [InlineData(4, 1, 30)]
    [InlineData(1, 2, 30)]
    [InlineData(2, 2, 30)]
    [InlineData(3, 2, 30)]
    [InlineData(4, 2, 30)]
    [InlineData(1, 3, 30)]
    [InlineData(2, 3, 30)]
    [InlineData(3, 3, 30)]
    [InlineData(4, 3, 30)]
    [InlineData(1, 4, 30)]
    [InlineData(2, 4, 30)]
    [InlineData(3, 4, 30)]
    [InlineData(4, 4, 30)]
    public void LongRunningTest(int threads, int method, int minutes)
    {
        LongRunMethod(threads, method, minutes);
    }

    private async void LongRunMethod(int threads, int method, int minutes)
    {
        Sudoku sudoku = new Sudoku();

        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(minutes));
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
