import Foundation

// blargh. Swift doesn't have 2D arrays so we emulate one with a flat array and maths
class TreeGrid : Sequence {
    func makeIterator() -> TreeGridIterator {
        TreeGridIterator(parent: self)
    }
    
    struct TreeGridIterator : IteratorProtocol {
        typealias Element = (row: Int, col: Int, score: Int)
        var row: Int = 0
        var col: Int = 0
        let parent: TreeGrid
        
        mutating func next() -> Self.Element? {
            row += 1
            if row == parent.rows {
                row = 0
                col += 1
            }
            if col == parent.cols {
                return nil // end of sequence
            }
            return (row, col, parent[row, col])
        }
    }
    
    var rawBuffer: [Int] // numbers between 0 and 9
    let rows: Int
    let cols: Int
    
    init(rows: Int, cols: Int) {
        self.rows = rows
        self.cols = cols
        self.rawBuffer = Array(repeating: Int(0), count: rows * cols)
    }
    
    func setRow(number: Int, values: [Int]) {
        if values.count != cols { fatalError("can't set row; doesn't have \(cols) items in it") }
        let startIdx = number * cols
        let endIdx = startIdx + cols
        
        rawBuffer.replaceSubrange(startIdx..<endIdx, with: values)
    }
    
    subscript(row: Int, col: Int) -> Int {
        get {
            if row < 0 || col < 0 { fatalError("row or col negative") }
            if row >= rows || col >= cols { fatalError("row or col too large") }
            
            return rawBuffer[row * cols + col]
        }
        set {
            if row < 0 || col < 0 { fatalError("row or col negative") }
            if row >= rows || col >= cols { fatalError("row or col too large") }
            
            rawBuffer[row * cols + col] = newValue
        }
    }
}


func parse(lines: [String]) -> TreeGrid {
    let grid = TreeGrid(rows: lines.count, cols: lines.first?.count ?? 0)
    
    var i = 0
    for line in lines {
        grid.setRow(number: i, values: line.map({ ch in Int(String(ch)) ?? 0 }))
        i += 1
    }
    
    return grid
}

// converts a TreeGrid where the numbers are heights into a same-sized array where the row,col entry is 1 if a tree is visible, 0 if not
func markVisibleTrees(grid: TreeGrid) -> TreeGrid {
    // initial grid is all zeroes so no trees are visible
    let visGrid = TreeGrid(rows: grid.rows, cols: grid.cols)
    
    // now let's run the algorithm.
    // there's almost certainly a fancy computer science way to do this with less code and big-O complexity
    // but I'm tired and brute force works fine for this
    for row in 0..<grid.rows {
        // left edge, drill towards the right
        // tree on the edge is always visible
        visGrid[row, 0] = 1
        var maxHeight = grid[row, 0]

        for col in 1..<grid.cols {
            if grid[row, col] > maxHeight {
                maxHeight = grid[row, col]
                visGrid[row, col] = 1
            }
        }

        // right edge, drill towards the left
        // tree on the edge is always visible
        let lastCol = grid.cols-1
        visGrid[row, lastCol] = 1
        maxHeight = grid[row, lastCol]

        for x in 1..<grid.cols {
            let col = lastCol-x

            if grid[row, col] > maxHeight {
                maxHeight = grid[row, col]
                visGrid[row, col] = 1
            }
        }
    }
    for col in 0..<grid.cols {
        // top edge, drill down
        // tree on the edge is always visible
        visGrid[0, col] = 1
        var maxHeight = grid[0, col]

        for row in 1..<grid.rows {
            if grid[row, col] > maxHeight {
                maxHeight = grid[row, col]
                visGrid[row, col] = 1
            }
        }

        // bottom edge, drill up
        // tree on the edge is always visible
        let lastRow = grid.rows-1
        visGrid[lastRow, col] = 1
        maxHeight = grid[lastRow, col]

        for x in 1..<grid.rows {
            let row = lastRow-x

            if grid[row, col] > maxHeight {
                maxHeight = grid[row, col]
                visGrid[row, col] = 1
            }
        }
    }
    return visGrid
}

private func scenicScore(trees: TreeGrid, row: Int, col: Int) -> Int {
    let leftScore: Int, rightScore: Int, upScore: Int, downScore: Int
    
    // drill left
    let maxHeight = trees[row, col]
    
    var treesSeen = 0
    var r = row
    while r > 0 {
        r -= 1
        treesSeen += 1
        if trees[r,col] >= maxHeight {
            break
        }
    }
    leftScore = treesSeen
    
    // drill right
    treesSeen = 0
    r = row
    while r < trees.rows-1 {
        r += 1
        treesSeen += 1
        if trees[r,col] >= maxHeight {
            break
        }
    }
    rightScore = treesSeen
    
    // drill up
    treesSeen = 0
    var c = col
    while c > 0 {
        c -= 1
        treesSeen += 1
        if trees[row,c] >= maxHeight {
            break
        }
    }
    upScore = treesSeen
    
    // drill down
    treesSeen = 0
    c = col
    while c < trees.cols-1 {
        c += 1
        treesSeen += 1
        if trees[row,c] >= maxHeight {
            break
        }
    }
    downScore = treesSeen
    
    let totalScore = leftScore * rightScore * upScore * downScore
    
//    print("for \(row),\(col): l=\(leftScore) * r=\(rightScore) * u=\(upScore) * d=\(downScore) ::==> \(totalScore)")
    return totalScore
}

// converts a TreeGrid where the numbers are heights into a same-sized array where the row,col entry is the scenic score of a tree
func computeScenicScores(grid: TreeGrid) -> TreeGrid {
    // initial grid is all zeroes so no trees are visible
    let scoreGrid = TreeGrid(rows: grid.rows, cols: grid.cols)
    
    for row in 0..<grid.rows {
        for col in 0..<grid.cols {
            scoreGrid[row, col] = scenicScore(trees: grid, row: row, col: col)
        }
    }
    return scoreGrid
}


struct Puzzle8TreetopTreeHouseP1 {
    
    public static func run(fileName: String) throws {
        let lines = try linesInFile(fileName)
        let grid = parse(lines: lines)
        print(grid.printRecursive())
        
        let visGrid = markVisibleTrees(grid: grid)
        print(visGrid.printRecursive(vis: true))
        
        let numTreesVisible = visGrid.rawBuffer.sum{ Int($0 )} // uint8 can only count to 255
        print("numTreesVisible = \(numTreesVisible)")
    }
}

struct Puzzle8TreetopTreeHouseP2 {
    
    public static func run(fileName: String) throws {
        let lines = try linesInFile(fileName)
        let grid = parse(lines: lines)
        print(grid.printRecursive())
        
        let scoresGrid = computeScenicScores(grid: grid)
        print(scoresGrid.printRecursive())
        
        let bestTree = scoresGrid.max { l, r in
            l.score < r.score
        }!
        print("best tree has score of \(bestTree.score) at \(bestTree.row),\(bestTree.col)")
    }
}

extension TreeGrid {
    func printRecursive(vis: Bool = false) -> String {
        var buf = ""
        for row in 0..<self.rows {
            for col in 0..<self.cols {
                let val = self[row, col]
                // if we're in "vis" mode, print a _ for a tree we can't see and a T for one we can
                // in normal mode we just print the numbers
                buf += vis ?
                    (val == 0 ? "_" : "T") :
                    String(val)
            }
            buf += "\n"
        }
        return buf
    }
}
