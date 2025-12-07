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
        var problems = new List<MathProblem>();
        // read the first line to work out the shape of the rest

        //using var e = ExampleInput.EnumerateLines().GetEnumerator();
        using var e = File.ReadLines("Day6Input.txt").GetEnumerator();
        if(!e.MoveNext()) throw new Exception("No first line in input?");
        var firstLineReader = new DelimitedLineReader(e.Current);
        while(firstLineReader.HasNonWhitespaceDataRemaining())
        {
            problems.Add(new MathProblem { Items = { firstLineReader.ReadSignedLong() } });
        }
        
        // now read the rest of the lines
        while (e.MoveNext())
        {
            var reader = new DelimitedLineReader(e.Current);

            for (int problemIdx = 0; problemIdx < problems.Count && reader.HasDataRemaining(); problemIdx++)
            {
                var problem = problems[problemIdx];
                
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
        
        // now action them all
        long grandTotal = 0;
        foreach (var problem in problems)
        {
            problem.PrintWithResult();
            
            var result = problem.Compute();
            grandTotal += result;
        }

        Console.WriteLine("Grand total is {0}", grandTotal);
    }
}