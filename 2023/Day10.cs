using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace aoc203;

internal class Day10
{
    public static readonly string ExampleInputBasicClean = """
        .....
        .S-7.
        .|.|.
        .L-J.
        .....
        """;

    public static readonly string ExampleInputBasicMessy = """
        -L|F7
        7S-7|
        L|7||
        -L-J|
        L|-JF
        """;

    public static readonly string ExampleInputComplexClean = """
        ..F7.
        .FJ|.
        SJ.L7
        |F--J
        LJ...
        """;

    public static readonly string ExampleInputComplexMessy = """
        7-F7-
        .FJ|7
        SJLL7
        |F--J
        LJ.LJ
        """;

    public static string LoadInput(InputSource inputSource) => inputSource switch
    {
        InputSource.Example => ExampleInputComplexMessy,
        InputSource.Real => File.ReadAllText("Day10-input.txt"),
        _ => throw new ArgumentException($"Can't handle inputSource {inputSource}")
    };

    // Note: this is a naturally recursive problem, and would be shorter that way, but I'm deliberately choosing to do it iteratively 
    // because the concept that every recursive solution can be written iteratively happens to be on my mind for some reason.
    // And also because I find it fun to optimize for perf/memory here; An iterative solution only hold the last element of each sequence in memory
    // whereas a recursive solution would implicitly keep the entire sequence in memory
    public static void Part1(InputSource inputSource)
    {
        var (field, startLocation) = ParseField(LoadInput(inputSource));

        // the question says that pipes can only ever have two connections, and the main 
        // loop is valid, therefore we can guarantee that a single step will always "work"
        // and we don't have to backtrack after hitting any dead-ends.

        // from the start location, pick the first edge (it doesn't matter how we define first)
        // and step continuously until we arrive back at the start location, gathering a trail of
        // tiles/points we encountered along the way and counting upwards.
        // Once we hit the start location, backtrack, counting upwards a second time, until our
        // counting backwards number equals our counting upwards number. That should be the farthest point

        var current = field[startLocation];
        var forwardsTrail = WalkPath(field, startLocation, current.ConnectsTo().Second);
        var backwardsTrail = WalkPath(field, startLocation, current.ConnectsTo().First);

        (Tile Tile, int DistanceFromStart)? mostDistant = null;
        for (int i = 0; i < forwardsTrail.Count; i++)
        {
            if (forwardsTrail[i] == backwardsTrail[i])
            {
                mostDistant = forwardsTrail[i];
                break;
            }
        }

        if (mostDistant == null) throw new Exception("Could not find most distant tile!");

        Console.WriteLine($"Most distant tile is: {mostDistant.Value.Tile} which is {mostDistant.Value.DistanceFromStart} steps away");

        var cleanPoints = forwardsTrail.Select(t => t.Tile.Location).ToHashSet();
        var cleanField = field.Where(kv => cleanPoints.Contains(kv.Key)).ToDictionary();

        // size of the field is determined before we clean it
        var maxX = field.Keys.Max(k => k.Y) + 1;
        var maxY = field.Keys.Max(k => k.Y) + 1;

        PrintField(cleanField, new Point(maxX, maxY));
    }

    static void PrintField(Dictionary<Point, Tile> field, Point size)
    {
        var (maxX, maxY) = size;

        for(int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                var tileType = field.TryGetValue(new Point(x, y), out var t) ? t.Type : '.';

                Console.Write(NiceChar(tileType));
            }
            Console.WriteLine();
        }

        static char NiceChar(char tileType) => tileType switch
        {
            '-' => '─',
            '|' => '│',
            '7' => '┐',
            'J' => '┘',
            'L' => '└',
            'F' => '┌',
            _ => tileType,
        };
    }

    // The "S" tile has two connections; `blockStartingConnectionTo` considers one of them as blocked, so the algorithm
    // will pick the other connection, thus you can control which direction it walks
    static List<(Tile Tile, int DistanceFromStart)> WalkPath(Dictionary<Point, Tile> field, Point startLocation, Point blockStartingConnectionTo)
    {
        var trail = new List<(Tile Tile, int DistanceFromStart)>();

        var current = field[startLocation];
        var distance = 0;
        Point currentLocation = startLocation;
        Point prevLocation = blockStartingConnectionTo;
        do
        {
            var currentAtStart = currentLocation;
            currentLocation = current.TraverseFrom(prevLocation);
            current = field[currentLocation];

            trail.Add((current, ++distance));
            prevLocation = currentAtStart;

        } while (current.Type != 'S');

        return trail;
    }

    static (Dictionary<Point, Tile> field, Point startLocation) ParseField(string input)
    {
        var result = new Dictionary<Point, Tile>();
        Point startLocation = new(-1, -1);

        int y = 0;
        foreach (var line in input.EnumerateLines())
        {
            int x = 0;
            foreach (var ch in line)
            {
                var pt = new Point(x++, y);
                result[pt] = new Tile(ch, pt);

                if (ch == 'S') startLocation = pt;
            }

            y++;
        }

        if (startLocation.X == -1) throw new FormatException("Invalid Field, did not contain starting tile S");

        return (result, startLocation);
    }

    record struct PointPair(Point First, Point Second) : IEnumerable<Point>
    {
        public IEnumerator<Point> GetEnumerator()
        {
            yield return First;
            yield return Second;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    record struct Point(int X, int Y);

    record Tile(char Type, Point Location)
    {
        public PointPair ConnectsTo() => Type switch
        {
            '-' => new(new(X - 1, Y), new(X + 1, Y)),
            '|' => new(new(X, Y - 1), new(X, Y + 1)),

            '7' => new(new(X - 1, Y), new(X, Y + 1)),
            'J' => new(new(X - 1, Y), new(X, Y - 1)),
            'L' => new(new(X, Y - 1), new(X + 1, Y)),
            // Note: In the examples and real input, the start point is an "F" tile so we can cheat and avoid having to work that out
            'F' or 'S' => new(new(X, Y + 1), new(X + 1, Y)),

            '.' => throw new Exception("This is an empty tile, don't call ConnectsTo()"),
            _ => throw new Exception($"Unhandled tile type {Type}, don't call ConnectsTo()"),
        };

        public Point TraverseFrom(Point startingPoint) => ConnectsTo().First(p => p != startingPoint);

        int X => Location.X;
        int Y => Location.Y;
    }

    // public static ReadOnlySpan<byte> TileValidTypes => ".|-LJ7F.S"u8;
}