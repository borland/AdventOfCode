using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

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

    public int MovePast(ReadOnlySpan<char> expectedText)
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

    public int ReadInt(out int into)
    {
        into = ReadInt();
        return CurrentPosition;
    }

    public readonly bool HasDataRemaining() => CurrentPosition < line.Length;

    // returns the character at the current position, then moves forward by 1
    public char ReadChar() => line[CurrentPosition++];
}
