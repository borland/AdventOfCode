import Foundation

fileprivate struct CleanupAssignments {
    let first: ClosedRange<Int>
    let second: ClosedRange<Int>
}

fileprivate func parseFile(fileName: String) throws -> [CleanupAssignments] {
    return try linesInFile(fileName)
        .map { line in
            let tokens = line.split(separator: ",")
            guard tokens.count == 2 else { fatalError("Can't deal with line \(line)") }
            let r1 = tokens[0].split(separator: "-").compactMap{ Int($0) }
            guard r1.count == 2 else { fatalError("Can't deal with range \(tokens[0])") }
            let r2 = tokens[1].split(separator: "-").compactMap{ Int($0) }
            guard r2.count == 2 else { fatalError("Can't deal with range \(tokens[1])") }
            
            return CleanupAssignments(first: r1[0]...r1[1], second: r2[0]...r2[1])
        }
}

struct Puzzle4CampCleanupP1 {
    
    public static func run(fileName: String) throws {
        let grandTotal = try parseFile(fileName: fileName)
            .map { assignments in
                let a = assignments.first, b = assignments.second
                // if one of the ranges fully contains the other, return 1 to 'count' it; else 0
                
                if (a.contains(b.lowerBound) && a.contains(b.upperBound)) ||
                    (b.contains(a.lowerBound) && b.contains(a.upperBound)) {
                    print("range contain hit for \(a) vs \(b)")
                    return 1
                }
                return 0
            }
            .reduce(0, +)
        
        print("Grand Total=\(grandTotal)")
    }
}

struct Puzzle4CampCleanupP2 {
    
    public static func run(fileName: String) throws {
        let grandTotal = try parseFile(fileName: fileName)
            .map { assignments in
                let a = assignments.first, b = assignments.second
                
                if a.overlaps(b) {
                    print("range contain hit for \(a) vs \(b)")
                    return 1
                }
                return 0
            }
            .reduce(0, +)
        
        print("Grand Total=\(grandTotal)")
    }
}

