using aoc203;
using System.Diagnostics;

//Day1.Part1(Day1.LoadInput());
//Day1.Part2(Day1.LoadInput());

//Day2.Part1(Day2.LoadInput());
//Day2.Part2(Day2.LoadInput());

//Day3.Part1(Day3.LoadInput());
//Day3.Part2(Day3.LoadInput());

//Day4.Part1(InputSource.Example);

//Day5.Part1(InputSource.Real);

//Day6.Part1(InputSource.Example);
//Day6.Part1(InputSource.Real);
//Day6.Part2(InputSource.Example);
//Day6.Part2(InputSource.Real);

Day7.Part1(InputSource.Example);
Day7.Part1(InputSource.Real);

var sw = Stopwatch.StartNew();

for (int i = 0; i < 500; i++)
{
    Day7.Part1(InputSource.Example);
    Day7.Part1(InputSource.Real);
}

sw.Stop();
Console.WriteLine("Completed in {0}ms", sw.ElapsedMilliseconds);

//Day7.Part2(InputSource.Example);
//Day7.Part2(InputSource.Real);