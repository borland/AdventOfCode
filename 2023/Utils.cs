namespace aoc203;

static class Utils
{
    public static IEnumerable<string> EnumerateLines(this string str)
    {
        using var reader = new StringReader(str);
        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
    }
}

ref struct DelimitedLineReader(ReadOnlySpan<char> line)
{
    readonly ReadOnlySpan<char> line = line;
    public int CurrentPosition { get; private set; }

    // the MovePast extension method is probably what you want instead of this
    public int MovePastReturningPosition(ReadOnlySpan<char> expectedText)
    {
        if (CurrentPosition + expectedText.Length > line.Length)
        {
            throw new FormatException($"Attempting MovePast {expectedText} but line was too short, only {line[CurrentPosition..]} remains");
        }

        var lineSubSpan = line[CurrentPosition..(CurrentPosition + expectedText.Length)];
        if (!lineSubSpan.Equals(expectedText, StringComparison.Ordinal))
        {
            throw new FormatException($"Attempting MovePast {expectedText} but found {lineSubSpan}");
        }

        CurrentPosition += expectedText.Length;
        return CurrentPosition;
    }

    // move forward until we hit a character that doesn't match 'predicate'
    public ReadOnlySpan<char> Scan(Func<char, bool> predicate)
    {
        var startIdx = CurrentPosition;
        for (int i = CurrentPosition; i < line.Length; i++)
        {
            if (!predicate(line[i]))
            {
                CurrentPosition = i;
                return line[startIdx..i];
            }
            // else keep scanning
        }
        // if we reach here it means we consumed the remainder of the line while satisfying predicate
        CurrentPosition = line.Length;
        return line[startIdx..];
    }

    public int Scan(Func<char, bool> predicate, out ReadOnlySpan<char> into)
    {
        into = Scan(predicate);
        return CurrentPosition;
    }

    public int ReadInt()
    {
        var span = Scan(char.IsAsciiDigit);
        return int.Parse(span);
    }

    public readonly bool HasDataRemaining() => CurrentPosition < line.Length;

    // returns the character at the current position, then moves forward by 1
    public char ReadChar() => line[CurrentPosition++];

    // returns the character at the current position, but doesn't move forward
    public char PeekChar() => line[CurrentPosition];
}

// Interesting. I want to use the builder pattern for DelimitedLineReader (where all the methods return `this` so you can chain them)
// but I also want the reader not to allocate anything on the heap, so we can create a reader-per-line extremely cheaply.
// `ref struct` achieves the latter, but if you do `return this` from within a ref struct, it makes a copy.
// 
// This workaround using extension methods seems to be the way to achieve the desired result.
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs8170?f1url=%3FappId%3Droslyn%26k%3Dk(CS8170)
// Essentially, we treat the whole thing as a byref type, except it's allocated on the stack.
// C++ here we come
static class DelimitedLineReaderExtensions
{
    public static ref DelimitedLineReader MovePast(this ref DelimitedLineReader reader, ReadOnlySpan<char> expectedText)
    {
        reader.MovePastReturningPosition(expectedText);
        return ref reader;
    }

    public static ref DelimitedLineReader Scan(this ref DelimitedLineReader reader, Func<char, bool> predicate, out ReadOnlySpan<char> into)
    {
        into = reader.Scan(predicate);
        return ref reader;
    }

    public static ref DelimitedLineReader ReadInt(this ref DelimitedLineReader reader, out int into)
    {
        into = reader.ReadInt();
        return ref reader;
    }
}