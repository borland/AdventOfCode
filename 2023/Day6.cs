using System.Diagnostics;

namespace aoc203;

class Day6
{
    public static readonly string ExampleInput = """
        Time:      7  15   30
        Distance:  9  40  200
        """;

    public static readonly string RealInput = """
        Time:        51     69     98     78
        Distance:   377   1171   1224   1505
        """;

    public static string LoadInput(InputSource inputSource) => inputSource switch
    {
        InputSource.Example => ExampleInput,
        InputSource.Real => RealInput,
        _ => throw new ArgumentException($"Can't handle inputSource {inputSource}")
    };

    public static readonly string ExampleInput2 = """
        Time:      71530
        Distance:  940200
        """;

    public static readonly string RealInput2 = """
        Time:        51699878
        Distance:   377117112241505
        """;

    public static string LoadInput2(InputSource inputSource) => inputSource switch
    {
        InputSource.Example => ExampleInput2,
        InputSource.Real => RealInput2,
        _ => throw new ArgumentException($"Can't handle inputSource {inputSource}")
    };

    public static void Part1(InputSource inputSource)
    {
        var races = ParseRaces(LoadInput(inputSource));
        var allWaysToWin = new List<int>();

        foreach(var race in races)
        {
            var waysToWin = Enumerable
                .Range(1, (int)race.TimeLimit - 2)
                .Select(i => DistanceTravelled(i, (int)race.TimeLimit - i))
                .Count(x => x > race.TargetDistance);

            Console.WriteLine("{0} ways to win", waysToWin);
            allWaysToWin.Add(waysToWin);
        }

        Console.WriteLine("{0} multipled all ways to win\n", allWaysToWin.Aggregate((a, b) => a * b));
    }

    public static void Part2(InputSource inputSource)
    {
        var races = ParseRaces(LoadInput2(inputSource));
        var allWaysToWin = new List<long>();

        var race = races.Single();

        // I was expecting this to be really slow and need some sort of optimization, but it takes 28ms on a release build in .NET 8
        var sw = Stopwatch.StartNew();

        var waysToWin = 0;
        for(int i = 1; i < race.TimeLimit - 1; i++)
        {
            var distance = DistanceTravelled(i, race.TimeLimit - i);
            if (distance > race.TargetDistance) waysToWin++;
        }

        sw.Stop();

        Console.WriteLine("{0} ways to win; took {1}ms\n", waysToWin, sw.ElapsedMilliseconds);
        allWaysToWin.Add(waysToWin);

    }

    static long DistanceTravelled(long timePushingButton, long timeMoving) => timePushingButton * timeMoving;    

    static IReadOnlyList<Race> ParseRaces(string input)
    {
        var times = new List<long>();
        var distances = new List<long>();

        foreach(var line in input.EnumerateLines())
        {
            var reader = new DelimitedLineReader(line);
            reader.ReadWord(out var type).MovePast(":");

            if(type == "Time")
            {
                while (reader.HasDataRemaining()) times.Add(reader.ReadLong());
            }
            else if(type == "Distance")
            {
                while (reader.HasDataRemaining()) distances.Add(reader.ReadLong());
            }
        }
        if (times.Count != distances.Count) throw new FormatException("Malformed file: Mismatching number of times and distances");

        return times.Zip(distances, (time, dist) => new Race(time, dist)).ToList();
    }

    record Race(long TimeLimit, long TargetDistance);
}