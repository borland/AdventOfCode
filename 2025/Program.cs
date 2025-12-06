using System.Diagnostics;

namespace aoc25;

static class Program
{
    static void Main(string[] args)
    {
        var start = Stopwatch.GetTimestamp();
        Day5.Run();
        var end = Stopwatch.GetElapsedTime(start);
        Console.WriteLine("Complete in {0}ms", end.TotalMilliseconds);
    }
}
