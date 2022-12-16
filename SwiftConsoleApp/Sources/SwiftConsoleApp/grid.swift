struct Point: Equatable {
    let x:Int
    let y:Int
    
    static func + (lhs: Point, rhs: Point) -> Point {
        Point(x: lhs.x + rhs.x, y: lhs.y + rhs.y)
    }
}

protocol Renderable {
    static var defaultValue: Self { get }
    
    func render() -> Character
}

class Grid<T> where T : Renderable {
    let rows: Int
    let cols: Int
    var rawBuffer: [T]
    
    init(rows: Int, cols: Int, initialValue: T = T.defaultValue) {
        self.rows = rows
        self.cols = cols
        self.rawBuffer = Array(repeating: initialValue, count: rows * cols)
    }
    
    // returns nil if you read an out of range value. fatalError if you try write out of range
    subscript(x: Int, y: Int) -> T? {
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
    
    subscript(point: Point) -> T? {
        get { self[point.x, point.y] }
        set { self[point.x, point.y] = newValue }
    }
    
    func render(frame: Frame?) -> String {
        if let f = frame {
            return render(x: f.x, y: f.y, width: f.width, height: f.height)
        } else {
            return render()
        }
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
                output.append(rawBuffer[i].render())
            }
            
            if newLinesToWrite > 0 && i % cols == cols-1 {
                newLinesToWrite -= 1
                output.append("\n")
            }
        }
        return output
    }
}

struct Frame {
    let x: Int
    let y: Int
    let width: Int
    let height: Int
}

// only renders the part of the grid that isn't defaultValue.
// requires that our grid contents are equatable otherwise we can't even tell.
extension Grid where T : Equatable {
    
    func getInterestingViewport() -> Frame? {
        var minX = cols, minY = rows, maxX = 0, maxY = 0
        
        for i in rawBuffer.indices {
            let x = i % cols
            let y = i / cols
            
            if rawBuffer[i] != T.defaultValue {
                if x < minX {
                    minX = x
                }
                if y < minY { minY = y }
                if x > maxX { maxX = x }
                if y > maxY { maxY = y }
            }
        }
        
        if maxX - minX > 0 && maxY - minY > 0 {
            // draw at least one unit of padding around the content unless it would cause us to overshoot
            var extraHeight = 2
            var extraWidth = 2
            if minX > 0 { minX -= 1; extraWidth = 2 }
            if minY > 0 { minY -= 1; extraHeight = 3 }
            
            return Frame(x: minX, y: minY, width: extraWidth + maxX - minX, height: extraHeight + maxY - minY)
        } else {
            return nil
        }
    }
}
