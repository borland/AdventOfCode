using System.ComponentModel;
using System.Linq;

namespace aoc203;

class Day3
{
    public static readonly string ExampleInput = """
        467..114..
        ...*......
        ..35..633.
        ......#...
        617*......
        .....+.58.
        ..592.....
        ......755.
        ...$.*....
        .664.598..
        """;

    public static string LoadInput() => File.ReadAllText("Day3-input.txt");

    public static void Part1(string input)
    {
        var tokens = ParseSchematic(input);

        // any number adjacent to a symbol, even diagonally, is a "part number" and should be included in your sum.

        // we know symbols are only of length 1
        var symbolLocations = tokens.OfType<SchemaToken.Symbol>().Select(t => new Point(t.Y, t.StartX)).ToHashSet();

        var partNumberTokens = new List<SchemaToken.Number>();

        foreach(var numberToken in tokens.OfType<SchemaToken.Number>()) 
        {
            var top = numberToken.Y - 1;
            var left = numberToken.StartX - 1;

            foreach (int yPos in Enumerable.Range(top, 3))
            { 
                foreach (int xPos in Enumerable.Range(left, numberToken.EndX - numberToken.StartX + 3)) // endX is inclusive so we need + 3
                {
                    if(symbolLocations.Contains(new Point(yPos, xPos)))
                    {
                        partNumberTokens.Add(numberToken);
                        goto nextToken;
                    }
                }
            }
        nextToken:;
        }

        Console.WriteLine(partNumberTokens.Sum(token => token.Value));
    }

    public static void Part2(string input)
    {
        var tokens = ParseSchematic(input);

        // any number adjacent to a symbol, even diagonally, is a "part number" and should be included in your sum.

        // in part 2 we only care about gears.
        // Key: Gear location
        // Value: List of part numbers adjacent to this Gear
        var gears = tokens.OfType<SchemaToken.Symbol>().Where(t => t.Value == '*').ToDictionary(t => new Point(t.Y, t.StartX), t => new List<int>());

        foreach (var numberToken in tokens.OfType<SchemaToken.Number>())
        {
            var top = numberToken.Y - 1;
            var left = numberToken.StartX - 1;

            foreach (int yPos in Enumerable.Range(top, 3))
            {
                foreach (int xPos in Enumerable.Range(left, numberToken.EndX - numberToken.StartX + 3)) // endX is inclusive so we need + 3
                {
                    if (gears.TryGetValue(new Point(yPos, xPos), out var adjacentParts) && !adjacentParts.Contains(numberToken.Value))
                    {
                        adjacentParts.Add(numberToken.Value);
                    }
                }
            }
        }

        var total = 0;
        foreach(var z in gears.Values.Where(l => l.Count == 2))
        {
            var gearRatio = z[0] * z[1];
            //Console.WriteLine("First gear is {0}, second gear is {1}, ratio is {2}", z[0], z[1], gearRatio);
            total += gearRatio;
        }
        Console.WriteLine("Sum of gear ratios is {0}", total);
    }

    static List<SchemaToken> ParseSchematic(string input)
    {
        List<SchemaToken> tokens = new();

        int y = 0;
        foreach(var line in input.EnumerateLines())
        {
            var reader = new DelimitedLineReader(line);

            while (reader.HasDataRemaining())
            {
                var currentChar = reader.PeekChar();
                if (char.IsAsciiDigit(currentChar)) // found a number
                {
                    var startX = reader.CurrentPosition;
                    var schemaNumber = reader.ReadInt();
                    var endX = reader.CurrentPosition-1;
                    tokens.Add(new SchemaToken.Number(y, startX, endX, schemaNumber));
                }
                else
                {
                    if (currentChar != '.')
                    {
                        tokens.Add(new SchemaToken.Symbol(y, reader.CurrentPosition, reader.CurrentPosition, currentChar));
                    }
                    reader.ReadChar(); // advance one
                }
            }
            y++;
        }

        return tokens;
    }

    record struct Point(int Y, int X);

    // EndX is inclusive, so a single-character token will have both StartX and EndX at the same value.
    internal abstract record SchemaToken(int Y, int StartX, int EndX)
    {
        internal record Number(int Y, int StartX, int EndX, int Value) : SchemaToken(Y, StartX, EndX);
        internal record Symbol(int Y, int StartX, int EndX, char Value) : SchemaToken(Y, StartX, EndX);
    }

    record Schematic(IReadOnlyList<SchemaToken> Tokens);
}
