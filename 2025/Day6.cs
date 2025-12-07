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

    public static void Run()
    {
        // Parse
        var problems = ParseProblems(ExampleInput.EnumerateLines());

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
        
        // Part 2: ridiculous BS
        // TODO stop. I can't just take the `long` values I parsed before; whitespace is significant now, I need to go back and re-parse the file. Sigh.
        foreach (var problem in problems.Select(p => ToCephalopodForm(p)))
        {
            problem.PrintWithResult();
            var result = problem.Compute();
            grandTotal += result;
        }

        Console.WriteLine("Part 2 Grand total is {0}", grandTotal);
    }

    // This is wrong. whitespace is significant now, I need to go back and re-parse the file. Sigh.
    static MathProblem ToCephalopodForm(MathProblem problem)
    {
        var numbersAsListsOfChar = problem.Items.Select(i => i.ToString().AsEnumerable().ToList()).ToList();
        var maxLength = numbersAsListsOfChar.Max(s => s.Count);

        // now walk backwards, combining all the chars
        var convertedProblem = new MathProblem { Operation = problem.Operation };
        for (int i = maxLength; i >= 0; i--)
        {
            var tmp = new List<char>();
            foreach (var n in numbersAsListsOfChar)
            {
                // if there's a digit in this column, add it to the buffer; else there's no digit so nothing to add
                if (i < n.Count) tmp.Add(n[i]);
            }
            convertedProblem.Items.Add(int.Parse(new string(tmp.ToArray())));
        }
        return convertedProblem;
    }

    static List<MathProblem> ParseProblems(IEnumerable<string> lines)
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