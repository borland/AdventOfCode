namespace aoc203;

class Day4
{
    public static readonly string ExampleInput = """
        Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
        Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
        Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
        Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
        Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
        Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
        """;

    public static string LoadInput(InputSource inputSource) => inputSource switch
    {
        InputSource.Example => ExampleInput,
        InputSource.Real => File.ReadAllText("Day4-input.txt"),
        _ => throw new ArgumentException($"Can't handle inputSource {inputSource}")
    };

    public static void Part1(InputSource inputSource)
    {
        var input = LoadInput(inputSource);
        var total = ParseScratchCards(input).Sum(card => card.PointValue());
        Console.WriteLine("Total = {0}", total);
    }

    static IEnumerable<ScratchCard> ParseScratchCards(string input)
    {
        foreach(var line in input.EnumerateLines())
        {
            var reader = new DelimitedLineReader(line);
            reader.MovePast("Card").ReadInt(out var cardId).MovePast(":");

            var winningNumbers = new HashSet<int>();
            var numbersYouHave = new List<int>();

            ICollection<int> activeList = winningNumbers;

            while (reader.HasDataRemaining())
            {
                var ch = reader.PeekChar();
                if (char.IsWhiteSpace(ch))
                {
                    reader.NextChar();
                }
                else if (char.IsAsciiDigit(ch))
                {
                    activeList.Add(reader.ReadInt());
                }
                else if (ch == '|')
                {
                    activeList = numbersYouHave;
                    reader.NextChar();
                }
            }
            yield return new ScratchCard(cardId, winningNumbers, numbersYouHave);
        }
    }

    record ScratchCard(int Id, IReadOnlySet<int> WinningNumbers, IReadOnlyList<int> NumbersYouHave)
    {
        public int PointValue()
        {
            var power = NumbersYouHave.Count(WinningNumbers.Contains);
            return (int)Math.Pow(2, power-1);
        }
    }
}
