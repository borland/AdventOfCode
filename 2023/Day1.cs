namespace aoc203;

class Day1
{
    public static readonly string Part1ExampleInput = """
        1abc2
        pqr3stu8vwx
        a1b2c3d4e5f
        treb7uchet
        """;

    public static string LoadInput() => File.ReadAllText("Day1-input.txt");

    static int ComputeCalibrationValue(ReadOnlySpan<char> line)
    {
        var digitChars = new List<char>();
        foreach(var ch in line)
        {
            if (char.IsAsciiDigit(ch)) digitChars.Add(ch);
        }

        // a single digit is both the first and last digiti
        if (digitChars.Count == 1) digitChars.Add(digitChars[0]);

        if (digitChars.Count < 2) throw new FormatException($"line {line} did not contain at least 2 digit characters");

        return int.Parse($"{digitChars[0]}{digitChars[^1]}");
    }

    public static void Part1(string input)
    {
        var total = 0;
        foreach (var line in input.EnumerateLines())
        {
            var calibrationValue = ComputeCalibrationValue(line);
            Console.WriteLine("Calibration value for {0} is {1}", line, calibrationValue);
            total += calibrationValue;
        }
        Console.WriteLine("Total is {0}", total);
    }

    public static readonly string Part2ExampleInput = """
        two1nine
        eightwothree
        abcone2threexyz
        xtwone3four
        4nineeightseven2
        zoneight234
        7pqrstsixteen
        """;

    static int ComputeCalibrationValueIncludingWords(ReadOnlySpan<char> line)
    {
        var digitChars = new List<char>();

        for (int i = 0; i < line.Length; i++)
        {
            // the data is sneaky, it overlaps words, so "oneight" counts as "one eight"
            if (char.IsAsciiDigit(line[i])) digitChars.Add(line[i]);
            else if (IsWordAt(line, i, "one")) digitChars.Add('1');
            else if (IsWordAt(line, i, "two")) digitChars.Add('2');
            else if (IsWordAt(line, i, "three")) digitChars.Add('3');
            else if (IsWordAt(line, i, "four")) digitChars.Add('4');
            else if (IsWordAt(line, i, "five")) digitChars.Add('5');
            else if (IsWordAt(line, i, "six")) digitChars.Add('6');
            else if (IsWordAt(line, i, "seven")) digitChars.Add('7');
            else if (IsWordAt(line, i, "eight")) digitChars.Add('8');
            else if (IsWordAt(line, i, "nine")) digitChars.Add('9');
        }

        // a single digit is both the first and last digiti
        if (digitChars.Count == 1) digitChars.Add(digitChars[0]);

        if (digitChars.Count < 2) throw new FormatException($"line {line} did not contain at least 2 digit characters");

        return int.Parse($"{digitChars[0]}{digitChars[^1]}");
    }

    static bool IsWordAt(ReadOnlySpan<char> line, int startIdx, ReadOnlySpan<char> word)
    {
        if (startIdx + word.Length > line.Length) return false; // fell off the end of the line
        return line[startIdx..(startIdx+word.Length)].Equals(word, StringComparison.Ordinal);
    }

    public static void Part2(string input)
    {
        var total = 0;
        foreach (var line in input.EnumerateLines())
        {
            var calibrationValue = ComputeCalibrationValueIncludingWords(line);
            Console.WriteLine("Calibration value for {0} is {1}", line, calibrationValue);
            total += calibrationValue;
        }
        Console.WriteLine("Total is {0}", total);
    }
}
