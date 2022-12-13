private enum ListEntry : CustomStringConvertible, Comparable {
    case int(Int)
    case list([ListEntry])
    
    var description: String {
        switch self {
        case .int(let i): return "\(i)"
        case .list(let l): return "[\(l.map({ $0.description }).joined(separator: ","))]"
        }
    }
    
    ///    If both values are integers, the lower integer should come first.
    ///    If the left integer is lower than the right integer, the inputs are in the right order. If the left integer is higher than the right integer, the inputs are not in the right order.
    ///    Otherwise, the inputs are the same integer; continue checking the next part of the input.
    ///
    ///    If both values are lists, compare the first value of each list, then the second value, and so on.
    ///    If the left list runs out of items first, the inputs are in the right order. If the right list runs out of items first, the inputs are not in the right order.
    ///    If the lists are the same length and no comparison makes a decision about the order, continue checking the next part of the input.
    ///
    ///    If exactly one value is an integer, convert the integer to a list which contains that integer as its only value, then retry the comparison.
    ///    For example, if comparing [0,0,0] and 2, convert the right value to [2] (a list containing 2); the result is then found by instead comparing [0,0,0] and [2].
    static func < (lhs: ListEntry, rhs: ListEntry) -> Bool {
        switch (lhs, rhs) {
        case (.int(let a), .int(let b)):
            return a < b
            
        case (.list(let a), .list(let b)):
            // compare things in sequence, bail early if we find a mismatch
            var aIter = a.makeIterator(), bIter = b.makeIterator()
            while true {
                guard let aElem = aIter.next(), let bElem = bIter.next() else { break } // end of loop, one or both of the sequences finished
                if aElem < bElem { return true }
                if bElem < aElem { return false }
            }
            
            // if we get here it means that all the compared elements were equal,
            // but we might not have compared them all if the lists didn't have the same number of elements.
            // Tie breaker is the shortest list
            return a.count < b.count
            
        case (.int, .list):
            return ListEntry.list([lhs]) < rhs
            
        case (.list, .int):
            return lhs < ListEntry.list([rhs])
        }
    }
}

private struct PacketPair {
    let first: [ListEntry]
    let second: [ListEntry]
}

private func parseLine(_ line: String) -> [ListEntry] {
    func parseList(iterator: inout String.Iterator) -> [ListEntry] {
        var listEntries = [ListEntry]()
        
        var numBuffer = ""
        func flushNumBuffer() {
            if !numBuffer.isEmpty, let i = Int(numBuffer) {
                numBuffer.removeAll(keepingCapacity: true)
                listEntries.append(.int(i))
            }
        }
        
        while true {
            guard let char = iterator.next() else { fatalError("unhandled end of line") }
            switch char {
            case "0"..."9":
                numBuffer.append(char)
            case ",":
                flushNumBuffer()
            case "[":
                listEntries.append(.list(parseList(iterator: &iterator)))
            case "]":
                flushNumBuffer()
                return listEntries
            default:
                fatalError("unhandled char \(char)")
            }
        }
    }
    
    var lineIter = line.makeIterator()
    guard let char = lineIter.next() else { fatalError("invalid empty line") } // file reader should have stripped all the empty lines
    guard char == "[" else { fatalError("invalid start of list \(char) - expecting [")}
    
    return parseList(iterator: &lineIter)
}

private func parseIntoPairs(lines: [String]) -> [PacketPair] {
    var result = [PacketPair]()
    var lineIter = lines.makeIterator()
    while true {
        guard let l1 = lineIter.next(), let l2 = lineIter.next() else {
            break // end of input
        }
        
        result.append(PacketPair(first: parseLine(l1), second: parseLine(l2)))
    }
    return result
}

struct Day13DistressSignalP1 {
    
    static func run(fileName: String) throws {
        let pairs = parseIntoPairs(lines: try linesInFile(fileName, includeEmpty: false)) // don't care about the newline separators, we just pick packets in A,B,A,B sequence
        
        var correctlyOrderedIndices = [Int]()
        
        for (offset, pair) in pairs.enumerated() {
            print("Compare \(pair.first) vs \(pair.second)")
            if ListEntry.list(pair.first) < ListEntry.list(pair.second) {
                print("Left side is smaller, so inputs are in the right order")
                correctlyOrderedIndices.append(offset + 1)
            } else {
                print("Right side is smaller, so inputs are not in the right order")
            }
        }
        
        print("correctlyOrderedIndices = \(correctlyOrderedIndices)")
        print("sum = \(correctlyOrderedIndices.sum())")
    }
}

struct Day13DistressSignalP2 {
    
    static func run(fileName: String) throws {
        let lines = try linesInFile(fileName, includeEmpty: false) // don't care about the newline separators, we just pick packets in A,B,A,B sequence
        
        let divider1 = ListEntry.list([.list([.int(2)])])
        let divider2 = ListEntry.list([.list([.int(6)])])
        
        let sortedPackets = (lines.map { ListEntry.list(parseLine($0)) } + [divider1, divider2]).sorted()
        
//        for p in sortedPackets {
//            print(p)
//        }
        
        var div1index: Int?, div2Index: Int?
        for (offset, p) in sortedPackets.enumerated() {
            if p == divider1 {
                div1index = offset + 1
            } else if p == divider2 {
                div2Index = offset + 1
            }
        }
        
        guard let i1 = div1index, let i2 = div2Index else {
            fatalError("can't find divider packets in sorted list")
        }
        print("i1 = \(i1), i2 = \(i2), decoder key = \(i1 * i2)")
    }
}
