namespace aoc24;

public static class Day1
{
    private static readonly string example =
        """
        3   4
        4   3
        2   5
        1   3
        3   9
        3   3
        """;
    
    public static void Run()
    {
        Part1(example);
        Part1(File.ReadAllText("day1.txt"));
        
        Part2(example);
        Part2(File.ReadAllText("day1.txt"));
    }

    // sum the distances between the two numbers on each line
    static void Part1(string input)
    {
        var left = new List<int>();
        var right = new List<int>();
        foreach (var reader in input.EnumerateDelimitedLines())
        {
            left.Add(reader.ReadInt());
            right.Add(reader.ReadInt());
        }
        
        left.Sort();
        right.Sort();
        var distances = left.Zip(right).Select(pair => Math.Abs(pair.Second - pair.First));

        Console.WriteLine(distances.Sum());
    }

    static void Part2(string input)
    {
        var left = new List<int>();
        var occurrences = new Dictionary<int, int>();
        foreach (var reader in input.EnumerateDelimitedLines())
        {
            left.Add(reader.ReadInt());
            var right = reader.ReadInt();

            if (occurrences.TryGetValue(right, out var count))
            {
                occurrences[right] = count + 1;
            }
            else
            {
                occurrences[right] = 1;
            }
        }
        
        var similarityScore = left.Aggregate(0, (memo, input) => memo + Similarity(input));

        Console.WriteLine(similarityScore);

        int Similarity(int leftValue) => occurrences.TryGetValue(leftValue, out int occurrenceCount) ? leftValue * occurrenceCount : 0;
    }
}