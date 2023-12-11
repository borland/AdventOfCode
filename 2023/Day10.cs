using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace aoc203;

internal class Day10
{
    public static readonly string ExampleInput = """
        .....
        .S-7.
        .|.|.
        .L-J.
        .....
        """;

    public static string LoadInput(InputSource inputSource) => inputSource switch
    {
        InputSource.Example => ExampleInput,
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

        var trail = new List<(Tile Tile, int DistanceFromStart)>();

        var current = field[startLocation];
        var distance = 0;
        do
        {
            var nextPoint = current.ConnectsTo().First;
            current = field[nextPoint];
            trail.Add((current, ++distance));

        } while (current.Type != 'S');
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

    record struct PointPair(Point First, Point Second);

    record struct Point(int X, int Y);

    record Tile(char Type, Point Location)
    {
        public PointPair ConnectsTo() => Type switch
        {
            '-' => new(new(X - 1, Y), new(X + 1, Y)),
            '|' => new(new(X, Y - 1), new(X, Y + 1)),

            '7' => new(new(X - 1, Y), new(X, Y + 1)),
            'J' => new(new(X - 1, Y), new(X, Y - 1)),
            'L' => new(new(X, Y - 1), new(X, Y + 1)),
            'F' => new(new(X, Y + 1), new(X + 1, Y)),

            '.' => throw new Exception("This is an empty tile, don't call ConnectsTo()"),
            _ => throw new Exception($"Unhandled tile type {Type}, don't call ConnectsTo()"),
        };

        int X => Location.X;
        int Y => Location.X;
    }

    // public static ReadOnlySpan<byte> TileValidTypes => ".|-LJ7F.S"u8;
}