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

    public static void Part1(InputSource inputSource)
    {
        var sequences = ParseSequences(LoadInput(inputSource));
        long totalCarries = 0;
        var sources = new Stack<long[]>();
        foreach (var s in sequences)
        {
            Console.WriteLine(string.Join(' ', s.Select(l => l.ToString())));

            var source = s;
            sources.Clear();
            while(true)
            {
                sources.Push(source);
                var deltas = new long[source.Length - 1];
                for (int i = 0; i < deltas.Length; i++)
                {
                    deltas[i] = source[i + 1] - source[i];
                }
                Console.WriteLine(string.Join(' ', deltas.Select(l => l.ToString())));

                if (deltas.Length == 0) throw new InvalidOperationException("Can't determine sequence");

                if (deltas.Sum() == 0) break;
                source = deltas;
            }

            // now walk back up the stack
            long delta = 0, carry = 0;
            while(sources.TryPop(out source))
            {
                carry = source[^1] + delta;
                Console.WriteLine("{0} delta:{1} => {2}", string.Join(' ', source.Select(l => l.ToString())), delta, carry);
                delta = carry;
            }

            totalCarries += carry;
            Console.WriteLine();
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
