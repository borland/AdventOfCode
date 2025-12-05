namespace aoc25;

public static class Day5
{
    private static readonly string ExampleInput =
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
                    if (reader.ReadChar() != '-') throw new InvalidOperationException("Expected hyphen");
                    var upper = reader.ReadLong();
                    freshRanges.Add(new IngredientRange(lower, upper));
                    break;

                case ParseState.IngredientIds:
                    ingredientIds.Add(reader.ReadLong());
                    break;
            }
        }

        int freshCount = 0;
        foreach (var ingredientId in ingredientIds)
        {
            var isFresh = (freshRanges.Any(r => r.ContainsInclusive(ingredientId)));
            if (isFresh)
            {
                freshCount++;
                Console.WriteLine("Ingredient ID {0} is fresh", ingredientId);    
            }
            else
            {
                Console.WriteLine("Ingredient ID {0} spoiled", ingredientId);
            }
        }

        Console.WriteLine("{0} of the ingredient IDs are fresh", freshCount);
    }
}

readonly record struct IngredientRange(long Lower, long Upper)
{
    public bool ContainsInclusive(long ingredientId) => ingredientId >= Lower && ingredientId <= Upper;
}