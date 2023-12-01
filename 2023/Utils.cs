using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc203;

static class Utils
{
    public static IEnumerable<string> EnumerateLines(this string str)
    {
        using var reader = new StringReader(str);
        while(reader.ReadLine() is { } line)
        {
            yield return line;
        }
    }
}
