namespace aoc25;

public static class Day6
{
#pragma warning disable CS0414 // Field is assigned but its value is never used
    private static readonly string ExampleInput =
#pragma warning restore CS0414 // Field is assigned but its value is never used
        """
        123 328  51 64 
         45 64  387 23 
          6 98  215 314
        *   +   *   +  
        """;

    enum Operation
    {
        None,
        Add,
        Multiply
    }

    static char OpChar(Operation operation) => operation switch
    {
        Operation.None => '?',
        Operation.Add => '+',
        Operation.Multiply => '*',
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
    };

    class MathProblem
    {
        public readonly List<long> Items = [];
        public Operation Operation = Operation.None;

        public long Compute()
        {
            return Operation switch
            {
                Operation.Add => Items.Sum(),
                Operation.Multiply => Items.Aggregate((x, y) => x * y),
                _ => throw new InvalidOperationException("Can't compute when Operation is None")
            };
        }

        // diagnostics only
        public void PrintWithResult()
        {
            Console.WriteLine(string.Join($" {OpChar(Operation)} ", Items) + $" = {Compute()}");
        }
    }

    public static void RunPart1()
    {
        // Parse
        var problems = ParseProblemsPart1(ExampleInput.EnumerateLines());

        // now action them all
        long grandTotal = 0;
        foreach (var problem in problems)
        {
            // problem.PrintWithResult();
            var result = problem.Compute();
            grandTotal += result;
        }

        Console.WriteLine("Part 1 Grand total is {0}", grandTotal);
        Console.WriteLine();
    }

    // part 2
    public static void Run()
    {
        // Parse
        var problems = ParseProblemsPart2(ExampleInput.EnumerateLines());

        // now action them all
        long grandTotal = 0;
        foreach (var problem in problems)
        {
            // problem.PrintWithResult();
            var result = problem.Compute();
            grandTotal += result;
        }

        Console.WriteLine("Part 2 Grand total is {0}", grandTotal);
        Console.WriteLine();
    }

    static List<MathProblem> ParseProblemsPart2(IEnumerable<string> lines)
    {
        // because the column boundaries could be anywhere, and we can't find them correctly without
        // seeing the entire dataset, we either have to read the whole input twice, or buffer it all in-memory
        // The input text file is small, let's just buffer.
        var allLines = lines.ToArray();
        List<int> columnStartPositions = FindColumnBoundaryPositions(allLines);
        var splitLines = allLines.Select(line => SplitAtBoundaries(line, columnStartPositions)).ToList();
        
        // now form problems out of the split lines
        var result = new List<MathProblem>();
        List<char> buf = new List<char>(capacity: 10);
        for (int tokenIdx = 0; tokenIdx < splitLines.Count; tokenIdx++)
        {
            var token = splitLines[tokenIdx];
            for (int charIdx = token.Count; charIdx >= 0; charIdx--)
            {
                if (token[charIdx] != ' ') buf.Add(token[charIdx]);
            }
            buf.Clear();
        }

        return [];
    }

    static List<int> FindColumnBoundaryPositions(string[] lines)
    {
        if (lines.Length == 0) throw new Exception("No lines found");

        // first column always starts at zero
        var results = new List<int>();

        // walk all lines at the same time. If a character is a space in all of them, it's a column boundary
        for (int i = 0; i < lines[0].Length; i++)
        {
            if (lines.All(l => l[i] == ' ')) results.Add(i);
        }

        // If there's double-whitespace do we need to disregard it? Assume the input doesn't have it
        return results;
    }

    static List<string> SplitAtBoundaries(string input, IEnumerable<int> columnBoundaryPositions)
    {
        var result = new List<string>();
        var prev = 0;
        foreach (var boundary in columnBoundaryPositions)
        {
            result.Add(input.Substring(prev, boundary - prev));
            prev = boundary + 1;
        }

        result.Add(input.Substring(prev, input.Length - prev));
        return result;
    }

    static List<MathProblem> ParseProblemsPart1(IEnumerable<string> lines)
    {
        var result = new List<MathProblem>();
        // read the first line to work out the shape of the rest

        using var e = lines.GetEnumerator();
        if (!e.MoveNext()) throw new Exception("No first line in input?");
        var firstLineReader = new DelimitedLineReader(e.Current);
        while (firstLineReader.HasNonWhitespaceDataRemaining())
        {
            result.Add(new MathProblem { Items = { firstLineReader.ReadSignedLong() } });
        }

        // now read the rest of the lines
        while (e.MoveNext())
        {
            var reader = new DelimitedLineReader(e.Current);

            for (int problemIdx = 0; problemIdx < result.Count && reader.HasDataRemaining(); problemIdx++)
            {
                var problem = result[problemIdx];

                // skip any leading whitespace. ReadSignedLong does this but PeekChar doesn't, let's help it out
                reader.Scan(char.IsWhiteSpace);
                var ch = reader.PeekChar();
                if (ch is '+' or '*') // we must be on the last row
                {
                    problem.Operation = ch switch
                    {
                        '+' => Operation.Add,
                        '*' => Operation.Multiply,
                        _ => throw new FormatException($"Can't parse operation {ch}")
                    };
                    reader.SkipChar();
                }
                else
                {
                    problem.Items.Add(reader.ReadSignedLong());
                }
            }
        }

        return result;
    }
}