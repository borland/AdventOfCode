using System.Collections;

namespace aoc25;

static class Utils
{
    extension(string str)
    {
        public IEnumerable<string> EnumerateLines()
        {
            using var reader = new StringReader(str);
            while (reader.ReadLine() is { } line)
            {
                yield return line;
            }
        }
    }

    // .NET9 now allows IEnumerable<ref struct>, but you can't use yield return with them
    // https://github.com/dotnet/roslyn/issues/75569#issuecomment-2429424385
    public static DelimitedLineReaderEnumerable EnumerateDelimitedLines(this string str) => new(str.EnumerateLines());

    public readonly ref struct DelimitedLineReaderEnumerable(IEnumerable<string> source) : IEnumerable<DelimitedLineReader>
    {
        public Enumerator GetEnumerator() => new(source.GetEnumerator());
        IEnumerator<DelimitedLineReader> IEnumerable<DelimitedLineReader>.GetEnumerator() => new Enumerator(source.GetEnumerator());
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        public readonly struct Enumerator(IEnumerator<string> inner) : IEnumerator<DelimitedLineReader>
        {
            public DelimitedLineReader Current => new(inner.Current);
            object IEnumerator.Current => throw new NotSupportedException();
            public void Dispose() => inner.Dispose();
            public bool MoveNext() => inner.MoveNext();
            public void Reset() => inner.Reset();
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

    // the Scan extension method is probably what you want instead of this
    public int ScanReturningPosition(Func<char, bool> predicate, out ReadOnlySpan<char> into)
    {
        into = Scan(predicate);
        return CurrentPosition;
    }

    // reads an integer, skipping past any leading whitespace
    public int ReadInt()
    {
        Scan(char.IsWhiteSpace); // skip past
        var span = Scan(char.IsAsciiDigit);
        return int.Parse(span);
    }

    // reads a long integer, skipping past any leading whitespace
    public long ReadLong()
    {
        Scan(char.IsWhiteSpace); // skip past
        var span = Scan(char.IsAsciiDigit);
        return long.Parse(span);
    }

    // reads a signed long integer, skipping past any leading whitespace
    public long ReadSignedLong()
    {
        Scan(char.IsWhiteSpace); // skip past

        bool isNegative = PeekChar() == '-';
        if (isNegative) ReadExactChar();

        var span = Scan(char.IsAsciiDigit);
        var num = long.Parse(span);

        return isNegative ? -num : num;
    }

    // reads a long integer, skipping past any leading whitespace
    public string ReadWord()
    {
        Scan(char.IsWhiteSpace); // skip past
        var span = Scan(char.IsAsciiLetter);
        return span.ToString();
    }
    
    // skips any leading whitespace, returns the character at that current position, then moves forward by 1.
    public char ReadChar()
    {
        Scan(char.IsWhiteSpace); // skip past
        return ReadExactChar();
    }

    // returns true if the Current Position is not at the end of the line.
    public readonly bool HasDataRemaining() => CurrentPosition < line.Length;

    // returns true if the Current Position is not at the end of the line, *and* there is no non-whitespace character after the current position.
    // this could be expensive if there are long runs of whitespace
    public readonly bool HasNonWhitespaceDataRemaining()
    {
        if (!HasDataRemaining()) return false;
        for (int pos = CurrentPosition; pos < line.Length; pos++)
        {
            if (!char.IsWhiteSpace(line[pos])) return true; // we found something that wasn't whitespace
        }
        return false;
    }

    // returns the character at the current position, then moves forward by 1.
    // does not skip past leading whitespace.
    public char ReadExactChar() => line[CurrentPosition++];

    // simply moves the current position forward by 1
    public void SkipChar() => CurrentPosition++;

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
internal static class DelimitedLineReaderExtensions
{
    extension(ref DelimitedLineReader reader)
    {
        public ref DelimitedLineReader MovePast(ReadOnlySpan<char> expectedText)
        {
            reader.MovePastReturningPosition(expectedText);
            return ref reader;
        }

        public ref DelimitedLineReader Scan(Func<char, bool> predicate, out ReadOnlySpan<char> into)
        {
            into = reader.Scan(predicate);
            return ref reader;
        }

        public ref DelimitedLineReader ReadInt(out int into)
        {
            into = reader.ReadInt();
            return ref reader;
        }

        public ref DelimitedLineReader ReadLong(out long into)
        {
            into = reader.ReadLong();
            return ref reader;
        }

        public ref DelimitedLineReader ReadSignedLong(out long into)
        {
            into = reader.ReadSignedLong();
            return ref reader;
        }

        public ref DelimitedLineReader ReadWord(out string into)
        {
            into = reader.ReadWord();
            return ref reader;
        }
    }
}