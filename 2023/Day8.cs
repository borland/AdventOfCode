using System.Collections.Frozen;
using System.Xml.Linq;

namespace aoc203;

internal class Day8
{
    public static readonly string ExampleInputA = """
        RL

        AAA = (BBB, CCC)
        BBB = (DDD, EEE)
        CCC = (ZZZ, GGG)
        DDD = (DDD, DDD)
        EEE = (EEE, EEE)
        GGG = (GGG, GGG)
        ZZZ = (ZZZ, ZZZ)
        """;

    public static readonly string ExampleInputB = """
        LLR

        AAA = (BBB, BBB)
        BBB = (AAA, ZZZ)
        ZZZ = (ZZZ, ZZZ)
        """;

    public static string LoadInput(InputSource inputSource) => inputSource switch
    {
        InputSource.Example => ExampleInputB,
        InputSource.Real => File.ReadAllText("Day8-input.txt"),
        _ => throw new ArgumentException($"Can't handle inputSource {inputSource}")
    };

    public static readonly string ExampleInputForPart2 = """
        LR

        11A = (11B, XXX)
        11B = (XXX, 11Z)
        11Z = (11B, XXX)
        22A = (22B, XXX)
        22B = (22C, 22C)
        22C = (22Z, 22Z)
        22Z = (22B, 22B)
        XXX = (XXX, XXX)
        """;

    public static string LoadInput2(InputSource inputSource) => inputSource switch
    {
        InputSource.Example => ExampleInputForPart2,
        InputSource.Real => File.ReadAllText("Day8-input.txt"),
        _ => throw new ArgumentException($"Can't handle inputSource {inputSource}")
    };

    public static void Part1(InputSource inputSource)
    {
        var (instructions, nodeDefinitions) = ParseNodes(LoadInput(inputSource));

        int steps = 0;

        if (!nodeDefinitions.TryGetValue("AAA", out var node)) throw new Exception("Can't find start node AAA");
        foreach (var instruction in RepeatForever(instructions))
        {
            node = instruction switch
            {
                Instruction.Left => nodeDefinitions[node.LeftNode],
                Instruction.Right => nodeDefinitions[node.RightNode],
                _ => throw new NotImplementedException(),
            };
            steps++;
            if (node.Name == "ZZZ") break;
        }

        Console.WriteLine($"Reached node ZZZ in {steps} steps");
    }

    public static void Part2(InputSource inputSource)
    {
        var (instructions, nodeDefinitions) = ParseNodes(LoadInput2(inputSource));

        long steps = 0;

        var nodes = nodeDefinitions.Values.Where(n => n.Name.EndsWith("A")).ToArray();

        foreach (var instruction in RepeatForever(instructions))
        {
            int endCount = 0;
            if (instruction == Instruction.Left)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i] = nodeDefinitions[nodes[i].LeftNode];
                    if (nodes[i].IsEndNode) endCount++;
                }

            }
            else if (instruction == Instruction.Right)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i] = nodeDefinitions[nodes[i].RightNode];
                    if (nodes[i].IsEndNode) endCount++;
                }
            }
            else
            {
                throw new NotImplementedException();
            }


            steps++;
            if (endCount == nodes.Length) break;
        }

        Console.WriteLine($"Reached all nodes ending with Z in {steps} steps");
    }

    enum Instruction { Left, Right };

    record Node(string Name, string LeftNode, string RightNode, bool IsEndNode);

    static (Instruction[] instructions, IDictionary<string, Node> nodes) ParseNodes(string input)
    {
        Instruction[]? instructions = null;
        Dictionary<string, Node> nodes = new();

        bool isFirstLine = true;
        foreach (var line in input.EnumerateLines())
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (isFirstLine)
            {
                instructions = line.Select(ch => ch switch
                {
                    'L' => Instruction.Left,
                    'R' => Instruction.Right,
                    _ => throw new ArgumentException($"must be L or R, got {ch}")
                }).ToArray();
                isFirstLine = false;
            }
            else
            {
                var reader = new DelimitedLineReader(line);
                reader
                    .Scan(char.IsAsciiLetterOrDigit, out var nodeName)
                    .MovePast(" = (")
                    .Scan(char.IsAsciiLetterOrDigit, out var leftNodeName)
                    .MovePast(", ")
                    .Scan(char.IsAsciiLetterOrDigit, out var rightNodeName)
                    .MovePast(")");

                var name = nodeName.ToString();

                nodes[name] = new Node(name, leftNodeName.ToString(), rightNodeName.ToString(), nodeName[^1] == 'Z'); // precompute whether it's an end node to make that quicker later
            }
        }

        if (instructions == null) throw new FormatException("Couldn't find instructions? Was the input empty?");

        return (instructions, nodes.ToFrozenDictionary());
    }

    static IEnumerable<T> RepeatForever<T>(IReadOnlyCollection<T> sequence)
    {
        while (true)
        {
            using var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }
}
