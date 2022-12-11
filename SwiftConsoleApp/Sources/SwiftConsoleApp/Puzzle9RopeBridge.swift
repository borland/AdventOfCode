import Foundation

// if head and tail are in the same space we just show head so no need to track both specifically
private enum RopePosition {
    case none, head, tail
}

private struct Point {
    let x: Int
    let y: Int
}

private struct Grid {
    let rows: Int
    let cols: Int
    var rawBuffer: [RopePosition]
    
    init(rows: Int, cols: Int) {
        self.rows = rows
        self.cols = cols
        self.rawBuffer = Array(repeating: .none, count: rows * cols)
    }
    
    subscript(x: Int, y: Int) -> RopePosition {
        get {
            if x < 0 || y < 0 || x >= cols || y >= rows { fatalError("x or y out of range") }
            return rawBuffer[y * cols + x]
        }
        set {
            if x < 0 || y < 0 || x >= cols || y >= rows { fatalError("x or y out of range") }
            rawBuffer[y * cols + x] = newValue
        }
    }
    
    subscript(point: Point) -> RopePosition {
        get { self[point.x, point.y] }
        set { self[point.x, point.y] = newValue }
    }
    
    func render() -> String {
        var output = ""
        for i in rawBuffer.indices {
            if rawBuffer[i] == .head { // head covers tail so draw "H" if they're in the same spot
                output += "H"
            } else if rawBuffer[i] == .tail {
                output += "T"
            } else {
                output += "."
            }
            
            if i % cols == cols-1 {
                output += "\n"
            }
        }
        return output
    }
}

private enum Instruction {
    case left(Int), right(Int), up(Int), down(Int)
    
    func microInstructions() -> [MicroInstruction] {
        switch self {
        case .left(let n): return Array(repeating: .left, count: n)
        case .right(let n): return Array(repeating: .right, count: n)
        case .up(let n): return Array(repeating: .up, count: n)
        case .down(let n): return Array(repeating: .down, count: n)
        }
    }
}

// an instruction to move but only ever by one position at a time
private enum MicroInstruction {
    case left, right, up, down
}

private func parseInstruction(text: String) -> Instruction {
    let components = text.split(separator: " ")
    guard components.count == 2, let num = Int(components[1]) else {
        fatalError("can't deal with line \(text)")
    }
    switch components[0] {
    case "L": return .left(num)
    case "R": return .right(num)
    case "U": return .up(num)
    case "D": return .down(num)
    default: fatalError("can't deal with line \(text)")
    }
}

struct Puzzle9RopeBridgeP1 {
    public static func run(fileName: String) throws {
        let lines = try linesInFile(fileName)
        
        var grid = Grid(rows: 10, cols: 10)
        let startPosition = Point(x: grid.cols / 2, y: grid.rows / 2)
        
        var headPos = startPosition
        var tailPos = startPosition
        
        grid[startPosition] = .head
        print(grid.render())

        // at the end of each frame, record where the tail was at the end of the frame and keep the record
        var tailTrailGrid = Grid(rows: grid.rows, cols: grid.cols)
        tailTrailGrid[startPosition] = .tail
        
        for instruction in lines.flatMap({ parseInstruction(text: $0).microInstructions() }) {
            switch instruction {
            case .left:
                headPos = Point(x: headPos.x-1, y: headPos.y)
            case .right:
                headPos = Point(x: headPos.x+1, y: headPos.y)
            case .up:
                headPos = Point(x: headPos.x, y: headPos.y-1)
            case .down:
                headPos = Point(x: headPos.x, y: headPos.y+1)
            }
            // now work out where the tail is supposed to go
            switch headPos.x - tailPos.x {
            case -1, 0, 1: break // tail is 0 or 1 space away from head, nothing to be done
            case 2:
                // head is 2 points to the right of tail, tail needs to move right and into the same column as head
                tailPos = Point(x: tailPos.x + 1, y: headPos.y)
            case -2:
                // head is 2 points to the left of tail, tail needs to move left and into the same column as head
                tailPos = Point(x: tailPos.x - 1, y: headPos.y)
            default: fatalError("somehow tail became detached from head")
            }
            
            switch headPos.y - tailPos.y {
            case -1, 0, 1: break // tail is 0 or 1 space away from head, nothing to be done
            case 2:
                // head is 2 points below of tail, tail needs to move down and into the same row as head
                tailPos = Point(x: headPos.x, y: tailPos.y + 1)
            case -2:
                // head is 2 points above of tail, tail needs to move up and into the same row as head
                tailPos = Point(x: headPos.x, y: tailPos.y - 1)
            default: fatalError("somehow tail became detached from head")
            }
            
            tailTrailGrid[tailPos] = .tail

//            var grid = Grid()
//            grid[tailPos] = .tail
//            grid[headPos] = .head
//            print(grid.render())
        }
        print(tailTrailGrid.render())
        
        let positionCount = tailTrailGrid.rawBuffer.reduce(0, { memo, pos in
            switch pos {
            case .tail: return memo + 1
            default: return memo
            }
        })
        
        print("tail visited \(positionCount) positions at least once")
    }
}

struct Puzzle9RopeBridgeP2 {
    public static func run(fileName: String) throws {
        let lines = try linesInFile(fileName)
    }
}
