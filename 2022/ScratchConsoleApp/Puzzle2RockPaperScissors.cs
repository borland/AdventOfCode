namespace ScratchConsoleApp;

enum Shape
{
    Rock,
    Paper,
    Scissors
}

enum Outcome
{
    Lose,
    Draw,
    Win
}

record struct RoundChoices(Shape OpponentChoice, Shape YourChoice);

static class Puzzle2Common
{
    public static int Score(this Shape shape) => shape switch
    {
        Shape.Rock => 1,
        Shape.Paper => 2,
        Shape.Scissors => 3,
        _ => throw new ArgumentException($"Unknown shape value {shape}")
    };

    public static int Score(this Outcome outcome) => outcome switch
    {
        Outcome.Lose => 0,
        Outcome.Draw => 3,
        Outcome.Win => 6,
        _ => throw new ArgumentException($"Unknown outcome value {outcome}")
    };

    public static Outcome Play(this RoundChoices round) => round switch
    {
        (Shape.Rock, Shape.Paper) or (Shape.Paper, Shape.Scissors) or (Shape.Scissors, Shape.Rock) => Outcome.Win,
        (Shape.Rock, Shape.Rock) or (Shape.Paper, Shape.Paper) or (Shape.Scissors, Shape.Scissors) => Outcome.Draw,
        (Shape.Rock, Shape.Scissors) or (Shape.Paper, Shape.Rock) or (Shape.Scissors, Shape.Paper) => Outcome.Lose,
        _ => throw new ArgumentException($"Unhandled entry {round}")
    };
}

public class Puzzle2RockPaperScissorsP1
{
    static IEnumerable<RoundChoices> ParseFile(string fileName)
    {
        foreach (var line in File.ReadLines(fileName))
        {
            var tokens = line.Split(" ", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2) throw new FormatException($"Unparseable line {line}");

            var opponentToken = tokens[0].ToUpperInvariant();
            var yourToken = tokens[1].ToUpperInvariant();
            yield return new RoundChoices(
                opponentToken switch
                {
                    "A" => Shape.Rock,
                    "B" => Shape.Paper,
                    "C" => Shape.Scissors,
                    _ => throw new FormatException($"Unparseable opponent choice {opponentToken}")
                },
                yourToken switch
                {
                    "X" => Shape.Rock,
                    "Y" => Shape.Paper,
                    "Z" => Shape.Scissors,
                    _ => throw new FormatException($"Unparseable your choice {yourToken}")
                });
        }
    }

    public static void Run(string fileName)
    {
        var grandTotal = ParseFile(fileName)
            .Select(entry =>
            {
                var shapeScore = entry.YourChoice.Score();
                var outcomeScore = entry.Play().Score();
                var totalScore = shapeScore + outcomeScore;
                Console.WriteLine(
                    $"Game: Opponent {entry.OpponentChoice}; You {entry.YourChoice}. ShapeScore={shapeScore}, OutcomeScore={outcomeScore}, Total={totalScore}");

                return totalScore;
            })
            .Sum();

        Console.WriteLine($"Grand Total={grandTotal}");
    }
}

public class Puzzle2RockPaperScissorsP2
{
    record struct GuideEntry(Shape OpponentChoice, Outcome DesiredOutcome);

    static IEnumerable<GuideEntry> ParseFile(string fileName)
    {
        foreach (var line in File.ReadLines(fileName))
        {
            var tokens = line.Split(" ", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2) throw new FormatException($"Unparseable line {line}");

            var opponentToken = tokens[0].ToUpperInvariant();
            var outcomeToken = tokens[1].ToUpperInvariant();
            yield return new GuideEntry(
                opponentToken switch
                {
                    "A" => Shape.Rock,
                    "B" => Shape.Paper,
                    "C" => Shape.Scissors,
                    _ => throw new FormatException($"Unparseable opponentToken {opponentToken}")
                },
                outcomeToken switch
                {
                    "X" => Outcome.Lose,
                    "Y" => Outcome.Draw,
                    "Z" => Outcome.Win,
                    _ => throw new FormatException($"Unparseable outcomeToken {outcomeToken}")
                });
        }
    }

    public static void Run(string fileName)
    {
        var grandTotal = ParseFile(fileName)
            .Select(entry =>
            {
                var yourChoice = entry switch
                {
                    // If the opponent chooses Rock, we want to Win, so we choose Paper
                    (Shape.Rock, Outcome.Win) => Shape.Paper,
                    (Shape.Rock, Outcome.Draw) => Shape.Rock,
                    (Shape.Rock, Outcome.Lose) => Shape.Scissors,

                    (Shape.Paper, Outcome.Win) => Shape.Scissors,
                    (Shape.Paper, Outcome.Draw) => Shape.Paper,
                    (Shape.Paper, Outcome.Lose) => Shape.Rock,

                    (Shape.Scissors, Outcome.Win) => Shape.Rock,
                    (Shape.Scissors, Outcome.Draw) => Shape.Scissors,
                    (Shape.Scissors, Outcome.Lose) => Shape.Paper,
                    _ => throw new ArgumentException($"Unhandled entry {entry}")
                };

                var totalScore = yourChoice.Score() + entry.DesiredOutcome.Score();
                Console.WriteLine(
                    $"Game: Opponent {entry.OpponentChoice}; Desired {entry.DesiredOutcome}; You {yourChoice}. Total={totalScore}");

                return totalScore;
            })
            .Sum();

        Console.WriteLine($"Grand Total={grandTotal}");
    }
}