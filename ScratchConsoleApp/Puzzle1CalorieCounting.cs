namespace ScratchConsoleApp;

public static class Puzzle1CalorieCounting
{
    record struct ElfInfo(int ElfNumber, int Calories);

    public static void Run(string inputFile) => PartTwo(inputFile);
    
    public static void Part1(string inputFile)
    {
        var elfNum = 1;
        var currentElfCalories = 0;
        bool lastLineWasBlank = false;

        var maxElf = new ElfInfo();
        
        void RecordCurrent()
        {
            Console.WriteLine($"Elf {elfNum} is carrying {currentElfCalories} calories");
            if (currentElfCalories > maxElf.Calories)
            {
                maxElf = new ElfInfo(elfNum, currentElfCalories);
            }
            
            elfNum++;
            currentElfCalories = 0;
        }
        
        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (string.IsNullOrWhiteSpace(line) && !lastLineWasBlank)
            {
                lastLineWasBlank = true; // accomodate multiple runs of blank lines

                RecordCurrent();
                continue;
            }

            if (int.TryParse(line, out var calories))
            {
                lastLineWasBlank = false;
                currentElfCalories += calories;
            }
            else
            {
                throw new Exception("Can't deal with line:\n" + line);
            }
        }

        RecordCurrent();

        Console.WriteLine($"The elf carrying the most calories is {maxElf.ElfNumber} with {maxElf.Calories} calories");
    }
    
    public static void PartTwo(string inputFile)
    {
        // keep the state machine to enumerate the file, but collect results into a list instead of just keeping maxElf
        var elfNum = 1;
        var currentElfCalories = 0;
        bool lastLineWasBlank = false;

        var allElves = new List<ElfInfo>(capacity: 300);
        
        void RecordCurrent()
        {
            Console.WriteLine($"Elf {elfNum} is carrying {currentElfCalories} calories");
            allElves.Add(new ElfInfo(elfNum, currentElfCalories));
            
            elfNum++;
            currentElfCalories = 0;
        }
        
        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (string.IsNullOrWhiteSpace(line) && !lastLineWasBlank)
            {
                lastLineWasBlank = true; // accomodate multiple runs of blank lines

                RecordCurrent();
                continue;
            }

            if (int.TryParse(line, out var calories))
            {
                lastLineWasBlank = false;
                currentElfCalories += calories;
            }
            else
            {
                throw new Exception("Can't deal with line:\n" + line);
            }
        }

        RecordCurrent();

        var top3Totalcalories = allElves.OrderByDescending(e => e.Calories).Take(3).Sum(e => e.Calories);
        
        Console.WriteLine($"The top 3 elves together carry {top3Totalcalories} calories");
    }
}