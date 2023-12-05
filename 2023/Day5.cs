using System.Collections.Generic;
using System.Text;

namespace aoc203;

class Day5
{
    public static readonly string ExampleInput = """
        seeds: 79 14 55 13

        seed-to-soil map:
        50 98 2
        52 50 48

        soil-to-fertilizer map:
        0 15 37
        37 52 2
        39 0 15

        fertilizer-to-water map:
        49 53 8
        0 11 42
        42 0 7
        57 7 4

        water-to-light map:
        88 18 7
        18 25 70

        light-to-temperature map:
        45 77 23
        81 45 19
        68 64 13

        temperature-to-humidity map:
        0 69 1
        1 0 69

        humidity-to-location map:
        60 56 37
        56 93 4
        """;

    public static string LoadInput(InputSource inputSource) => inputSource switch
    {
        InputSource.Example => ExampleInput,
        InputSource.Real => File.ReadAllText("Day5-input.txt"),
        _ => throw new ArgumentException($"Can't handle inputSource {inputSource}")
    };

    public static void Part1(InputSource inputSource)
    {
        var almanac = ParseAlmanac(LoadInput(inputSource));

        var currentMap = almanac.Maps.Single(m => m.From == "seed");
        var currentValues = almanac.Seeds;

        // keep going until we've processed all the maps
        while(currentMap != null)
        {
            currentValues = currentValues.Select(v => ConvertItem(currentMap.Entries, v)).ToList();

            // next stage; deliberate crash if we can't find one, that means the input is malformed
            currentMap = almanac.Maps.SingleOrDefault(m => m.From == currentMap.To);
        }

        Console.WriteLine("Lowest location number is {0}", currentValues.Min());
    }

    // I used this to debug the parsing process and ensure that I wasn't missing something. It's fine
    //static void PrintReconstructedAlmanac(Almanac almanac)
    //{
    //    var sb = new StringBuilder();
    //    sb.AppendLine($"seeds: {string.Join(" ", almanac.Seeds.Select(s => s.ToString()))}");
    //    sb.AppendLine();
    //    foreach(var map in almanac.Maps)
    //    {
    //        sb.AppendLine($"{map.From}-to-{map.To} map:");
    //        foreach(var entry in map.Entries)
    //        {
    //            sb.AppendLine($"{entry.DestinationRangeStart} {entry.SourceRangeStart} {entry.RangeLength}");
    //        }
    //        sb.AppendLine();
    //    }

    //    File.WriteAllText("C:\\Dev\\AdventOfCode\\2023\\Day5-reconstructed.txt", sb.ToString());
    //}

    static long ConvertItem(IReadOnlyList<MapEntry> mapEntries, long sourceNumber)
    {
        // SingleOrDefault because the instructions don't tell us how to deal with an overlapping range, throw if we hit one
        var applicableRange = mapEntries
            .SingleOrDefault(e => sourceNumber >= e.SourceRangeStart && sourceNumber < (e.SourceRangeStart + e.RangeLength));

        // Any source numbers that aren't mapped correspond to the same destination number
        if (applicableRange == null) return sourceNumber;

        var offsetInRange = sourceNumber - applicableRange.SourceRangeStart;
        return applicableRange.DestinationRangeStart + offsetInRange;
    }

    static Almanac ParseAlmanac(string input)
    {
        List<long> seeds = [];
        List<Map> maps = [];

        List<MapEntry>? activeMapEntries = null;

        foreach (var line in input.EnumerateLines())
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var reader = new DelimitedLineReader(line);
            if (line.StartsWith("seeds: "))
            {
                reader.MovePast("seeds: ");
                while (reader.HasDataRemaining()) seeds.Add(reader.ReadLong());
            }
            else if(line.EndsWith(" map:"))
            {
                reader
                    .ReadWord(out var from)
                    .MovePast("-to-")
                    .ReadWord(out var to);

                activeMapEntries = [];
                maps.Add(new Map(from, to, activeMapEntries));
            }
            else
            {
                if (activeMapEntries == null) throw new Exception($"Unexpected line {line} before any map was declared");
                reader
                    .ReadLong(out var destinationRangeStart)
                    .ReadLong(out var sourceRangeStart)
                    .ReadLong(out var rangeLength);

                activeMapEntries.Add(new MapEntry(destinationRangeStart, sourceRangeStart, rangeLength));
            }
        }

        return new Almanac(seeds, maps);
    }

    record Almanac(
        IReadOnlyList<long> Seeds,
        IReadOnlyList<Map> Maps);

    record Map(string From, string To, IReadOnlyList<MapEntry> Entries);

    record MapEntry(long DestinationRangeStart, long SourceRangeStart, long RangeLength);

}
