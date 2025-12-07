namespace aoc25;

public static class Day5
{
#pragma warning disable CS0414 // Field is assigned but its value is never used
    private static readonly string ExampleInput =
#pragma warning restore CS0414 // Field is assigned but its value is never used
        """
        3-5
        10-14
        16-20
        12-18

        1
        5
        8
        11
        17
        32
        """;

    enum ParseState
    {
        FreshRanges,
        IngredientIds
    };

    public static void Run()
    {
        List<IngredientRange> freshRanges = [];
        List<long> ingredientIds = [];

        var parseState = ParseState.FreshRanges;
        //foreach (var line in ExampleInput.EnumerateLines())
        foreach (var line in File.ReadLines("Day5Input.txt"))
        {
            var reader = new DelimitedLineReader(line);
            switch (parseState)
            {
                case ParseState.FreshRanges:
                    if (!reader.HasDataRemaining()) // it's the blank line
                    {
                        parseState = ParseState.IngredientIds;
                        break;
                    }

                    var lower = reader.ReadLong();
                    if (reader.ReadExactChar() != '-') throw new InvalidOperationException("Expected hyphen");
                    var upper = reader.ReadLong();
                    freshRanges.Add(new IngredientRange(lower, upper));
                    break;

                case ParseState.IngredientIds:
                    ingredientIds.Add(reader.ReadLong());
                    break;
            }
        }

        // Part 1
        var mergedRanges = MergeRanges(freshRanges);
        int freshCount = 0;
        foreach (var ingredientId in ingredientIds)
        {
            var isFresh = (mergedRanges.Any(r => r.ContainsInclusive(ingredientId)));
            if (isFresh) freshCount++;
        }

        Console.WriteLine("{0} of the ingredient IDs are fresh", freshCount);
        
        // Part 2. Collapse the ranges and sum their lengths.
        // Console.WriteLine("Ranges after merging:");
        long totalRangeLength = 0;
        foreach (var range in mergedRanges)
        {
            //Console.WriteLine(range);
            totalRangeLength += range.Length;
        }
        Console.WriteLine("{0} ingredient IDs are considered to be fresh", totalRangeLength);
    }

    static List<IngredientRange> MergeRanges(IEnumerable<IngredientRange> inputRanges)
    {
        var rangesSortedByStart = new List<IngredientRange>(inputRanges);
        rangesSortedByStart.Sort((a, b) => a.Lower.CompareTo(b.Lower));

        int startPos = 0;
        while (true)
        {
            bool didMerge = false;
            for (int i = startPos; i < rangesSortedByStart.Count-1; i++)
            {
                var a = rangesSortedByStart[i];
                var b = rangesSortedByStart[i+1];
                if (a.OverlapsInclusive(b))
                {
                    // Console.WriteLine("Range {0} overlaps {1}", a, b);
                    didMerge = true;
                    rangesSortedByStart.RemoveAt(i+1);
                    rangesSortedByStart[i] = a.Union(b);
                    startPos = i; // optimization. If we have non-mergeable stuff at the start it's going to stay non-mergeable so we can skip past it on subsequent iterations
                    break;
                    // merge them and restart the loop
                }
                // Console.WriteLine("Range {0} doesn't overlap {1}", a, b);
            }
            // we didn't manage to merge anything, must have reached the end
            if (!didMerge) break;
        }

        return rangesSortedByStart;
    }
}

readonly record struct IngredientRange(long Lower, long Upper)
{
    public bool ContainsInclusive(long ingredientId) => ingredientId >= Lower && ingredientId <= Upper;
    public bool OverlapsInclusive(IngredientRange other) => (Upper >= other.Lower && Lower <= Upper) || (Lower <= other.Upper && Upper >= other.Lower);
    
    public override string ToString() => $"{Lower}-{Upper}";
    
    public long Length => Upper + 1 - Lower; // 3-5 has a length of 3 as it includes 3 ID's 
    
    public IngredientRange Union(IngredientRange other) => new(Math.Min(Lower, other.Lower), Math.Max(Upper, other.Upper));
}