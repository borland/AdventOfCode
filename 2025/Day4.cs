using System.Collections;
using System.Diagnostics;

namespace aoc25;

public static class Day4
{
#pragma warning disable CS0414 // Field is assigned but its value is never used
    private static readonly string ExampleInput =
#pragma warning restore CS0414 // Field is assigned but its value is never used
        """
        ..@@.@@@@.
        @@@.@.@.@@
        @@@@@.@.@@
        @.@@@@..@.
        @@.@@@@.@@
        .@@@@@@@.@
        .@.@.@.@@@
        @.@@@.@@@@
        .@@@@@@@@.
        @.@.@@@.@.
        """;

    public static void Run()
    {
        //var grid = Grid.Parse(ExampleInput.EnumerateLines());
        var grid = Grid.Parse(File.ReadLines("Day4Input.txt"));

        int totalRollsRemoved = 0;
        while (true)
        {
            var nextGrid = Grid.Create(numColumns: grid.Columns, numRows: grid.Rows, fillWith: '.');

            var reachablePositions = 0;
            foreach (var point in grid)
            {
                nextGrid[point] = grid[point];

                if (grid[point] == '@')
                {
                    var adjacentRollCount = grid.CountAdjacent(point, '@');
                    if (adjacentRollCount < 4)
                    {
                        nextGrid[point] = '.'; // we removed the roll!
                        reachablePositions++;
                    }
                }
            }

            Console.WriteLine("{0} rolls were removed in this iteration", reachablePositions);
            if (reachablePositions == 0) break; // can't remove any more rolls, we're done
            totalRollsRemoved += reachablePositions;
            grid = nextGrid;
        }

        Console.WriteLine("A total of {0} rolls of paper were removed", totalRollsRemoved);

        // nextGrid.WriteToConsole();
    }

    public static void RunPart1()
    {
        //var grid = Grid.Parse(ExampleInput.EnumerateLines());
        var grid = Grid.Parse(File.ReadLines("Day4Input.txt"));

        // for printing, so I can see where I've gone wrong
        var outputGrid = Grid.Create(numColumns: grid.Columns, numRows: grid.Rows, fillWith: '.');

        var reachablePositions = 0;
        foreach (var point in grid)
        {
            outputGrid[point] = grid[point];

            if (grid[point] == '@')
            {
                var adjacentRollCount = grid.CountAdjacent(point, '@');
                if (adjacentRollCount < 4)
                {
                    outputGrid[point] = 'x';
                    reachablePositions++;
                }
            }
        }

        Console.WriteLine("{0} rolls can be accessed", reachablePositions);
        outputGrid.WriteToConsole();
    }

    readonly record struct Point(int X, int Y);

    class Grid(char[] buffer, int numColumns, int numRows) : IEnumerable<Point>
    {
        public static Grid Create(int numColumns, int numRows, char fillWith)
        {
            var buffer = new char[numColumns * numRows];
            Array.Fill(buffer, fillWith);
            return new Grid(buffer, numColumns, numRows);
        }

        public static Grid Parse(IEnumerable<string> lines)
        {
            var numColumns = 0;
            var numRows = 0;
            List<char> allLinesBuffer = []; // we don't know how many lines there are going to be
            foreach (var line in lines)
            {
                numRows++;
                if (numColumns == 0) numColumns = line.Length; // all lines should have the same length so just pick the first
                allLinesBuffer.AddRange(line.AsSpan());
            }

            Debug.Assert(allLinesBuffer.Count == numColumns * numRows);

            return new(allLinesBuffer.ToArray(), numColumns, numRows);
        }

        public int Columns => numColumns;
        public int Rows => numRows;

        public char this[Point point]
        {
            get => buffer[point.Y * numRows + point.X];
            set => buffer[point.Y * numRows + point.X] = value;
        }

        public int CountAdjacent(Point pos, char match)
        {
            Span<Point> candidates = stackalloc Point[8];

            var yUp = pos.Y - 1;
            var yDown = pos.Y + 1;
            int i = 0;

            if (yUp >= 0)
            {
                if (pos.X > 0) candidates[i++] = new(pos.X - 1, yUp);
                candidates[i++] = new(pos.X, yUp);
                if (pos.X < numColumns - 1) candidates[i++] = new(pos.X + 1, yUp);
            }

            if (pos.X > 0) candidates[i++] = new(pos.X - 1, pos.Y);
            // don't include pos itself, which would be here
            if (pos.X < numColumns - 1) candidates[i++] = new(pos.X + 1, pos.Y);

            if (yDown < numColumns)
            {
                if (pos.X > 0) candidates[i++] = new(pos.X - 1, yDown);
                candidates[i++] = new(pos.X, yDown);
                if (pos.X < numColumns - 1) candidates[i++] = new(pos.X + 1, yDown);
            }

            int count = 0;
            foreach (var candidate in candidates[..i])
            {
                if (this[candidate] == match) count += 1;
            }

            return count;
        }

        public void WriteToConsole()
        {
            for (int y = 0; y < numRows; y++)
            {
                for (int x = 0; x < numColumns; x++)
                {
                    Console.Write(buffer[y * numColumns + x]);
                }

                Console.WriteLine();
            }
        }

        public IEnumerator<Point> GetEnumerator()
        {
            for (int y = 0; y < numRows; y++)
            {
                for (int x = 0; x < numColumns; x++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}