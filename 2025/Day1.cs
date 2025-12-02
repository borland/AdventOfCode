namespace aoc25;

public static class Day1
{
    public static readonly string ExampleInput =
        """
        L68
        L30
        R48
        L5
        R60
        L55
        L1
        L99
        R14
        L82
        """;

    public static void Run()
    {
        const int dialPositionCount = 100;

        int dialPosition = 50;

        var zeroCount = 0;
        var zeroClickCount = 0;
        Console.WriteLine("The dial starts by pointing at {0}", dialPosition);

        foreach (var line in File.ReadAllLines("Day1Input.txt"))
        //foreach (var line in ExampleInput.EnumerateLines())
        {
            var reader = new DelimitedLineReader(line);
            var leftOrRight = reader.ReadChar();
            var originalAmount = reader.ReadInt();
            var amount = originalAmount;
            
            // super brute force
            switch (leftOrRight)
            {
                case 'L':
                    for (; amount > 0; --amount)
                    {
                        --dialPosition;
                        if (dialPosition < 0) dialPosition += dialPositionCount;
                        if (dialPosition == 0) ++zeroClickCount;
                    }
                   
                    break;
                case 'R':
                    for (; amount > 0; --amount)
                    {
                        ++dialPosition;
                        if (dialPosition >= dialPositionCount) dialPosition -= dialPositionCount;
                        if (dialPosition == 0) ++zeroClickCount;
                    }
                   
                    break;
                default:
                    throw new NotSupportedException(leftOrRight.ToString());
            }
            if (dialPosition == 0) ++zeroCount;
            Console.WriteLine("The dial is rotated {0}{1} to point at {2}.", leftOrRight, originalAmount, dialPosition);
        }
        
        Console.WriteLine("ZeroCount is {0}.", zeroCount);
        Console.WriteLine("ZeroClickCount is {0}.", zeroClickCount);
    }
}