//
//  File.swift
//  
//
//  Created by Orion Edwards on 8/12/22.
//

import Foundation

class TreeGrid {
    var rawBuffer: [UInt8] // numbers between 0 and 9
    let rows: Int
    let cols: Int
    
    init(rows: Int, cols: Int) {
        self.rows = rows
        self.cols = cols
        self.rawBuffer = Array(repeating: UInt8(0), count: rows * cols)
    }
    
    func setRow(number: Int, values: [UInt8]) {
        if values.count != cols { fatalError("can't set row; doesn't have \(cols) items in it") }
        let startIdx = number * cols
        let endIdx = startIdx + cols
        
        rawBuffer.replaceSubrange(startIdx..<endIdx, with: values)
    }
    
    subscript(row: Int, col: Int) -> UInt8 {
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

// converts a TreeGrid where the numbers are heights into a same-sized array where the row,col entry is 1 if a tree is visible, 0 if not
func markVisibleTrees(grid: TreeGrid) -> TreeGrid {
    // initial grid is all zeroes so no trees are visible
    let visGrid = TreeGrid(rows: grid.rows, cols: grid.cols)
    
    // now let's run the algorithm.
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

func parse(lines: [String]) -> TreeGrid {
    let grid = TreeGrid(rows: lines.count, cols: lines.first?.count ?? 0)
    
    var i = 0
    for line in lines {
        grid.setRow(number: i, values: line.map({ ch in UInt8(String(ch)) ?? 0 }))
        i += 1
    }
    
    return grid
}

struct Puzzle8TreetopTreeHouseP1 {
    
    public static func run(fileName: String) throws {
        let lines = try linesInFile(fileName)
        let grid = parse(lines: lines)
        print(grid.printRecursive())
        
        let visGrid = markVisibleTrees(grid: grid)
        print(visGrid.printRecursive())
        
        let numTreesVisible = visGrid.rawBuffer.sum{ Int($0 )} // uint8 can only count to 255
        print("numTreesVisible = \(numTreesVisible)")
    }
}

extension TreeGrid {
    func printRecursive() -> String {
        var buf = ""
        for row in 0..<self.rows {
            for col in 0..<self.cols {
                buf += String(self[row, col])
            }
            buf += "\n"
        }
        return buf
    }
}
