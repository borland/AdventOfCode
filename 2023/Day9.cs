namespace aoc203;

internal class Day9
{
    public static readonly string ExampleInput = """
        0 3 6 9 12 15
        1 3 6 10 15 21
        10 13 16 21 30 45
        """;

    public static string LoadInput(InputSource inputSource) => inputSource switch
    {
        InputSource.Example => ExampleInput,
        InputSource.Real => File.ReadAllText("Day9-input.txt"),
        _ => throw new ArgumentException($"Can't handle inputSource {inputSource}")
    };

    // Note: this is a naturally recursive problem, and would be shorter that way, but I'm deliberately choosing to do it iteratively 
    // because the concept that every recursive solution can be written iteratively happens to be on my mind for some reason.
    // And also because I find it fun to optimize for perf/memory here; An iterative solution only hold the last element of each sequence in memory
    // whereas a recursive solution would implicitly keep the entire sequence in memory
    public static void Part1(InputSource inputSource)
    {
        var sequences = ParseSequences(LoadInput(inputSource));
    
        long totalCarries = 0;
        var lastElements = new Stack<long>(); // memory-perf golfing: Don't need to allocate a new stack each time
        foreach (var s in sequences)
        {
            var source = s;
            lastElements.Clear();
            while (true)
            {
                lastElements.Push(source[^1]);
                var deltas = new long[source.Length - 1];
                for (int i = 0; i < deltas.Length; i++)
                {
                    deltas[i] = source[i + 1] - source[i];
                }

                if (deltas.Length == 0) throw new InvalidOperationException("Can't determine sequence");

                if (deltas.All(l => l == 0)) break;
                source = deltas;
            }

            // now walk back up the stack
            long delta = 0, carry = 0;
            long[]? lastSource = null;
            while (lastElements.TryPop(out var lastElement))
            {
                carry = lastElement + delta;
                //Console.WriteLine("{0} delta:{1} => {2}", string.Join(' ', source.Select(l => l.ToString())), delta, carry);
                delta = carry;
            }

            if (lastSource != null) Console.WriteLine("{0} *{1}*", string.Join(' ', lastSource.Select(l => l.ToString())), carry);

            totalCarries += carry;
            //Console.WriteLine();
        }
        Console.WriteLine("Total carries = {0}", totalCarries);
    }

    public static void Part2(InputSource inputSource)
    {
        var sequences = ParseSequences(LoadInput(inputSource));

        long totalCarries = 0;
        var firstElements = new Stack<long>(); // memory-perf golfing: Don't need to allocate a new stack each time
        foreach (var s in sequences)
        {
            //Console.WriteLine(string.Join(' ', s.Select(l => l.ToString())));

            var source = s;
            while (true)
            {
                firstElements.Push(source[0]);
                var deltas = new long[source.Length - 1];
                for (int i = 0; i < deltas.Length; i++)
                {
                    deltas[i] = source[i + 1] - source[i];
                }
                //Console.WriteLine(string.Join(' ', deltas.Select(l => l.ToString())));

                if (deltas.Length == 0) throw new InvalidOperationException("Can't determine sequence");

                if (deltas.All(l => l == 0)) break;
                source = deltas;
            }

            // now walk back up the stack
            long delta = 0, carry = 0;
            long[]? lastSource = null;
            while (firstElements.TryPop(out var firstElement))
            {
                lastSource = source;
                carry = firstElement - delta;
                //Console.WriteLine("*{2}* {0} delta:{1}", string.Join(' ', source.Select(l => l.ToString())), delta, carry);
                delta = carry;
            }

            totalCarries += carry;
        }
        Console.WriteLine("Total carries = {0}", totalCarries);
    }

    static IEnumerable<long[]> ParseSequences(string input)
    {
        foreach (var line in input.EnumerateLines())
        {
            var reader = new DelimitedLineReader(line);
            var buf = new List<long>();
            while (reader.HasDataRemaining()) buf.Add(reader.ReadSignedLong());
            yield return buf.ToArray();
        }
    }

}
