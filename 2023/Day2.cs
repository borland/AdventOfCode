using System.Diagnostics.CodeAnalysis;

namespace aoc203;

class Day2
{
    public static readonly string ExampleInput = """
        Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
        Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
        Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
        Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
        Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
        """;

    public static string LoadInput() => File.ReadAllText("Day2-input.txt");

    public static void Part1(string input)
    {
        var total = 0;
        foreach (var line in input.EnumerateLines())
        {
            var game = ParseGame(line);
            if (GameIsPossible(game, new CubeSet(12, 13, 14)))
            {
                Console.WriteLine("Game {0} is possible", game.Id);
                total += game.Id;
            }

        }
        Console.WriteLine("Total is {0}", total);
    }

    static string GameToString(Game game)
        => $"Game {game.Id}: {string.Join("; ", game.CubeSets.Select(s => $"{s.Red} red, {s.Green} green, {s.Blue} blue"))}";

    static bool GameIsPossible(Game game, CubeSet bagContents)
    {
        foreach (var set in game.CubeSets)
        {
            if (set.Red > bagContents.Red || set.Green > bagContents.Green || set.Blue > bagContents.Blue) return false;
        }
        return true;
    }

    static Game ParseGame(ReadOnlySpan<char> line)
    {
        var reader = new DelimitedLineReader(line);

        reader.MovePast("Game ").ReadInt(out var gameId).MovePast(": ");

        var sets = new List<CubeSet>();
        int red = 0, green = 0, blue = 0;
        while (reader.HasDataRemaining())
        {
            reader.ReadInt(out int n).MovePast(" ").Scan(char.IsAsciiLetterLower, out var color);

            switch (color)
            {
                case "red": red = n; break;
                case "green": green = n; break;
                case "blue": blue = n; break;
                default: throw new FormatException($"Unhandled color {color}");
            }

            if(reader.HasDataRemaining())
            {
                var ch = reader.ReadChar();
                switch (ch)
                {
                    case ';':
                        reader.Scan(char.IsWhiteSpace);
                        sets.Add(new CubeSet(red, green, blue));
                        red = green = blue = 0;
                        break;
                    case ',':
                        reader.Scan(char.IsWhiteSpace);
                        break;
                    default:
                        throw new FormatException($"Unexpected nextChar {ch}");
                }
            }
        }
        sets.Add(new CubeSet(red, green, blue)); // don't forget the last item
        return new Game(gameId, sets);
    }

    static Game ParseGameWithSplits(string line)
    {
        var outerComponents = line.Split(':', StringSplitOptions.TrimEntries);
        if (outerComponents.Length != 2 || !outerComponents[0].StartsWith("Game ")) throw new FormatException("Expected Line to be in format 'Game N: CubeSets'");
        if (!int.TryParse(outerComponents[0].AsSpan(5), out var gameId)) throw new FormatException("Can't parse gameId");

        var sets = outerComponents[1].Split(';', StringSplitOptions.TrimEntries);
        var cubeSets = new CubeSet[sets.Length];

        for (int i = 0; i < sets.Length; i++)
        {
            var set = sets[i];

            var setComponents = set.Split(',');
            int red = 0, green = 0, blue = 0;
            foreach (var s in setComponents)
            {
                if (s.EndsWith(" green")) green = int.Parse(s.AsSpan(0, s.Length - 6));
                else if (s.EndsWith(" blue")) blue = int.Parse(s.AsSpan(0, s.Length - 5));
                else if (s.EndsWith(" red")) red = int.Parse(s.AsSpan(0, s.Length - 4));
                else throw new FormatException($"can't parse setComponent [{s}] from set {set}");
            }

            cubeSets[i] = new CubeSet(red, green, blue);
        }

        return new Game(gameId, cubeSets);
    }

    record CubeSet(int Red, int Green, int Blue);
    record Game(int Id, IReadOnlyList<CubeSet> CubeSets);

    public static void Part2(string input)
    {
        var total = 0;
        foreach(var line in input.EnumerateLines())
        {
            var game = ParseGame(line);
            var m = MinimumPossibleCubes(game.CubeSets);
            var power = m.Red * m.Green * m.Blue;
            Console.WriteLine("Game {0}: Minimums: {1} red, {2} green, {3} blue. Power: {4}", game.Id, m.Red, m.Green, m.Blue, power);
            total += power;

        }
        Console.WriteLine("Total is {0}", total);
    }

    static CubeSet MinimumPossibleCubes(IEnumerable<CubeSet> cubeSets)
    {
        int maxRed = 0, maxGreen = 0, maxBlue = 0;
        foreach (var (r, g, b) in cubeSets)
        {
            if (r > maxRed) maxRed = r;
            if (g > maxGreen) maxGreen = g;
            if (b > maxBlue) maxBlue = b;
        }
        return new(maxRed, maxGreen, maxBlue);
    }
}