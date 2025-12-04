namespace aoc25;

public static class Day3
{
    private static readonly string ExampleInput =
        """
        987654321111111
        811111111111119
        234234234234278
        818181911112111
        """;
    
    public static void Run()
    {
        // You'll need to find the largest possible joltage each bank can produce. In the above example:
        // In 818181911112111, the largest joltage you can produce is 92.

        long total = 0;
        //foreach (var bank in ExampleInput.EnumerateLines())
        foreach (var bank in File.ReadLines("Day3Input.txt"))
        {
            // alg: Find the index of the largest number, that should be the first battery to turn on
            // then find the index of the largest number _after that_, which should be the second battery to turn on.
            // what if there are two copies of the largest number? Easy, turn them both on
            // what if the largest number is the last one? We can't use that, so constrain the first search to length-1
            var (firstBattery, firstIdx) = LargestAndIndex(bank, 0, bank.Length-1);
            var (secondBattery, _) = LargestAndIndex(bank, firstIdx+1, bank.Length);

            Console.WriteLine("In {0}, we can produce {1}{2}", bank, firstBattery, secondBattery);
            total += (firstBattery - '0') * 10 + (secondBattery - '0');
        }

        Console.WriteLine("Total: {0}", total);
    }

    static (char, int) LargestAndIndex(string bank, int startIndex, int endIndex)
    {
        char largest = '0';
        int indexOfLargest = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            if (bank[i] > largest)
            {
                largest = bank[i];
                indexOfLargest = i;
            }
        }

        return (largest, indexOfLargest);
    }
}