namespace aoc25;

public class Day1
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

    enum Direction
    {
        L,
        R
    }

    public static void Run()
    {
        const int dialPositionCount = 100;

        int dialPosition = 50;

        var zeroCount = 0;
        Console.WriteLine("The dial starts by pointing at {0}", dialPosition);

        foreach (var line in File.ReadAllLines("Day1Input.txt"))
        {
            var reader = new DelimitedLineReader(line);
            var leftOrRight = reader.ReadChar();
            var amount = reader.ReadInt();
            switch (leftOrRight)
            {
                case 'L':
                    dialPosition -= amount;
                    while (dialPosition < 0) dialPosition += dialPositionCount;
                    break;
                case 'R':
                    dialPosition += amount;
                    while (dialPosition >= dialPositionCount) dialPosition -= dialPositionCount;
                    break;
                default:
                    throw new NotSupportedException(leftOrRight.ToString());
            }
            Console.WriteLine("The dial is rotated {0}{1} to point at {2}.", leftOrRight, amount, dialPosition);
            if (dialPosition == 0) ++zeroCount;
        }
        
        Console.WriteLine("ZeroCount is {0}.", zeroCount);
    }
}