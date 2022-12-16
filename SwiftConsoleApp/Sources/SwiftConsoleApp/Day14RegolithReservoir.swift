private struct Point: Equatable {
    let x:Int
    let y:Int
    
    static func + (lhs: Point, rhs: Point) -> Point {
        Point(x: lhs.x + rhs.x, y: lhs.y + rhs.y)
    }
}
private enum Entity {
    case air, rock, sand, sandSource
}

private struct Grid {
    let rows: Int
    let cols: Int
    var rawBuffer: [Entity]
    
    init(rows: Int, cols: Int) {
        self.rows = rows
        self.cols = cols
        self.rawBuffer = Array(repeating: .air, count: rows * cols)
    }
    
    // returns nil if you read an out of range value. fatalError if you try write out of range
    subscript(x: Int, y: Int) -> Entity? {
        get {
            if x < 0 || y < 0 || x >= cols || y >= rows { return nil }
            return rawBuffer[y * cols + x]
        }
        set {
            if x < 0 || y < 0 || x >= cols || y >= rows { fatalError("x or y out of range") }
            if let entity = newValue {
                rawBuffer[y * cols + x] = entity
            }
        }
    }
    
    subscript(point: Point) -> Entity? {
        get { self[point.x, point.y] }
        set { self[point.x, point.y] = newValue }
    }
    
    func render(x minX:Int = 0, y minY:Int = 0, width:Int = 0, height:Int = 0) -> String {
        let maxX = width == 0 ? self.cols : minX + width
        let maxY = height == 0 ? self.rows : minY + height
        
        var newLinesToWrite = maxY - minY
        
        var output = ""
        for i in rawBuffer.indices {
            let x = i % cols
            let y = i / cols
            
            if x >= minX && y >= minY && x < maxX && y < maxY {
                switch rawBuffer[i] {
                case .air:
                    output += "."
                case .rock:
                    output += "#"
                case .sand:
                    output += "o"
                case .sandSource:
                    output += "+"
                }
            }
            
            if newLinesToWrite > 0 && i % cols == cols-1 {
                newLinesToWrite -= 1
                output += "\n"
            }
        }
        return output
    }
}

private func parse(line: String) -> [Point] {
    let components = line.components(separatedBy: " -> ")
    
    var result = [Point]()
    
    for c in components {
        let xy = c.split(separator: ",")
        guard xy.count == 2, let x = Int(xy[0]), let y = Int(xy[1]) else { fatalError("\(c) was not Int,Int") }
        
        result.append(Point(x: x, y: y))
    }
    return result
}

private func draw(path: [Point], onto grid: inout Grid) {
    var pathIter = path.makeIterator()
    guard var sourcePt = pathIter.next(), var destPt = pathIter.next() else {
        // nothing to draw unless there's 2 or more points
        return
    }
    
    while true {
        // draw from p1 to p2
        var nextPt = sourcePt
        while nextPt != destPt {
            grid[nextPt] = .rock
            
            let xDiff = destPt.x > nextPt.x ? 1 : (destPt.x < nextPt.x ? -1 : 0)
            let yDiff = destPt.y > nextPt.y ? 1 : (destPt.y < nextPt.y ? -1 : 0)
            
            nextPt = Point(x: nextPt.x + xDiff, y: nextPt.y + yDiff)
        }
        grid[nextPt] = .rock
        
        sourcePt = destPt
        guard let nextDest = pathIter.next() else { break } // end of path
        destPt = nextDest
    }
}

// simulates sand falling. returns nil if we fall off the edge of the grid
private func sandFall(from: Point, on grid: Grid) -> Point? {
    enum Fallable {
        case free, blocked, outOfBounds
    }
    func fallable(_ x: Int, _ y: Int) -> Fallable {
        if let target = grid[x, y] {
            return target == .air ? .free : .blocked
        } else {
            return .outOfBounds
        }
    }
    
    var (x, y) = (from.x, from.y)
    while true { // assume we start in a sensible place
        switch (fallable(x-1, y+1), fallable(x, y+1), fallable(x+1, y+1)) {
        case (_, .free, _):
            // we can fall directly down.
            y = y + 1
        case (.free, .blocked, _):
            // we can't fall directly down but we can fall to the left
            x = x - 1
            y = y + 1
        case (.blocked, .blocked, .free):
            // we can't fall down or left but we can fall to the right
            x = x + 1
            y = y + 1
        case (.blocked, .blocked, .blocked):
            // we can't fall at all. Stop here
            return Point(x: x, y: y)
        default:
            // other things will be out of bounds
            return nil
        }
    }
}

struct Day14RegolithReservoirP1 {
    
    static func run(fileName: String) throws {
        let sandSource = Point(x: 500, y: 0)
        
        var grid = Grid(rows: 550, cols: 550)
        grid[sandSource] = .sandSource
            
        for line in try linesInFile(fileName) {
            let path = parse(line: line)
            draw(path: path, onto: &grid)
        }
        
//        print(grid.render(x: 487, y: 0, width: 25, height: 11))
        
        for i in 0..<1000 {
            guard let sandPos = sandFall(from: sandSource, on: grid) else {
                // fell off the bottom of the grid. We're done here
                print("came to rest after \(i) units of sand")
                break
            }
            grid[sandPos] = .sand
        }
        
        print(grid.render(x:405, y: 0, width: 120, height: 170))
        
    }
}
