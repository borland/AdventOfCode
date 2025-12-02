namespace aoc25;

public static class Day2
{
    private static readonly string ExampleInput = "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";
    private static readonly string FullInput = "67562556-67743658,62064792-62301480,4394592-4512674,3308-4582,69552998-69828126,9123-12332,1095-1358,23-48,294-400,3511416-3689352,1007333-1150296,2929221721-2929361280,309711-443410,2131524-2335082,81867-97148,9574291560-9574498524,648635477-648670391,1-18,5735-8423,58-72,538-812,698652479-698760276,727833-843820,15609927-15646018,1491-1766,53435-76187,196475-300384,852101-903928,73-97,1894-2622,58406664-58466933,6767640219-6767697605,523453-569572,7979723815-7979848548,149-216";

    public static void Run()
    {
        var reader = new DelimitedLineReader(FullInput);
        long sumOfInvalidIds = 0;
        while (true)
        {
            var rangeLower = reader.ReadLong();
            if (reader.ReadChar() != '-') throw new Exception("Invalid input, expected '-'");
            var rangeUpper = reader.ReadLong();

            var invalidIds = FindInvalidIds(rangeLower, rangeUpper).ToArray();
            //Console.WriteLine("{0}-{1} has {2} invalid IDs: {3}",
            //    rangeLower, rangeUpper, invalidIds.Length, string.Join(", ", invalidIds));

            foreach (var invalidId in invalidIds)
            {
                sumOfInvalidIds += invalidId;
            }

            if (!reader.HasDataRemaining()) break; // the last ID doesn't have a trailing comma

            if (reader.ReadChar() != ',') throw new Exception("Invalid input, expected ','");
        }

        Console.WriteLine("Sum of invalid IDs = {0}", sumOfInvalidIds);
    }

    static IEnumerable<long> FindInvalidIds(long rangeLower, long rangeUpper)
    {
        // enumerable.Range doesn't work with longs
        for (var l = rangeLower; l <= rangeUpper; l++)
        {
            var str = l.ToString();
            if (!IdIsValidPart2(str)) yield return l;
        }
    }
    
    static bool IdIsValidPart1(string str)
    {
        if (str.Length % 2 != 0) return true; // Invalid ID's must have a sequence repeated twice, therefore a string with an odd number of digits can't be invalid
        
        var tupleLength = str.Length / 2;
        var front = str.AsSpan(0, tupleLength);
        var back = str.AsSpan(tupleLength, tupleLength);
        return !front.SequenceEqual(back); // ID is invalid, it contained a repeating set of digits
    }
    
    static bool IdIsValidPart2(string str)
    {
        for (var tupleLength = 1; tupleLength <= str.Length / 2; tupleLength++)
        {
            // we can't have repeating patterns if we can't divide the string up into tuples of equal length, so it must be valid (for this tuple length)
            if (str.Length % tupleLength != 0) continue;

            var startIdx = 0;
            var front = str.AsSpan(startIdx, tupleLength);
            
            while (startIdx < str.Length - tupleLength)
            {
                startIdx += tupleLength;
                var next = str.AsSpan(startIdx, tupleLength);
                if (!front.SequenceEqual(next))
                {
                    // this tuple did not match the first block, can't be a repeating pattern
                    goto continueOuterLoop;
                }
            }

            // if we get here it means the above while loop successfully matched all blocks as repeats of 'front'
            // and the ID must be invalid
            return false;
            
            continueOuterLoop: ;
        }

        return true;
    }
}