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

    public static void RunPart1()
    {
        // You'll need to find the largest possible joltage each bank can produce. In the above example:
        // In 818181911112111, the largest joltage you can produce is 92.

        long total = 0;
        foreach (var bank in ExampleInput.EnumerateLines())
            //foreach (var bank in File.ReadLines("Day3Input.txt"))
        {
            // alg: Find the index of the largest number, that should be the first battery to turn on
            // then find the index of the largest number _after that_, which should be the second battery to turn on.
            // what if there are two copies of the largest number? Easy, turn them both on
            // what if the largest number is the last one? We can't use that, so constrain the first search to length-1
            var (firstBattery, firstIdx) = LargestAndIndex(bank, 0, bank.Length - 1);
            var (secondBattery, _) = LargestAndIndex(bank, firstIdx + 1, bank.Length);

            Console.WriteLine("In {0}, we can produce {1}{2}", bank, firstBattery, secondBattery);
            total += (firstBattery - '0') * 10 + (secondBattery - '0');
        }

        Console.WriteLine("Total: {0}", total);
    }

    public static void Run()
    {
        // In 234234234234278, the largest joltage can be found by turning on everything except a 2 battery, a 3 battery, and another 2 battery near the start to produce 434234234278.
        // In 818181911112111, the joltage 888911112111 is produced by turning on everything except some 1s near the front.
        const int numBatteriesToTurnOn = 12;

        long total = 0;
        //foreach (var bank in ExampleInput.EnumerateLines())
            foreach (var bank in File.ReadLines("Day3Input.txt"))
        {
            // Should be similar to the first, except we constrain the 'end' to be -12, then -11, etc
            char[] batteries = new char[numBatteriesToTurnOn];
            int prevBatteryIdx = -1;
            for (int i = 0; i < numBatteriesToTurnOn; i++)
            {
                var endIdx = bank.Length - numBatteriesToTurnOn + i + 1;
                var (battery, idx) = LargestAndIndex(bank, prevBatteryIdx + 1, endIdx);
                batteries[i] = battery;
                prevBatteryIdx = idx;
            }

            var batteryString = new string(batteries);
            Console.WriteLine("In {0}, we can produce {1}", bank, batteryString);
            total += long.Parse(batteryString);
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