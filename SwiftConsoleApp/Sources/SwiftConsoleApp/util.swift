import Foundation

struct ParseError : LocalizedError {
    let message: String

    var errorDescription: String? { message }
}

extension StringProtocol where Index == String.Index {
    func trimmed() -> String {
        self.trimmingCharacters(in: .whitespaces)
    }
}

func linesInFile(_ fileName: String, includeEmptyLines: Bool = false) throws -> [String] {
    let contents = try String(contentsOfFile: fileName, encoding: .utf8)
    let includingEmptyLines = contents
        .split(separator: "\n", omittingEmptySubsequences: false)
        .map { line in line.trimmed() }
    
    return includeEmptyLines ?
        includingEmptyLines :
        includingEmptyLines.filter { line in line != "" }
}

extension Sequence {
    func distinct() -> [Element] where Element : Hashable {
        var items = Set<Element>()
        var result = [Element]()
        
        for item in self {
            let (inserted, _) = items.insert(item)
            if inserted {
                result.append(item)
            }
        }
        return result
    }
    
    func inGroupsOf(_ n:Int) -> [[Element]] {
        var outer = [[Element]]()
        
        var inner = [Element]()
        inner.reserveCapacity(n)
        
        for item in self {
            inner.append(item)
            if inner.count == n {
                outer.append(inner)
                inner.removeAll(keepingCapacity: true)
            }
        }
        
        if !inner.isEmpty {
            outer.append(inner)
        }
        return outer
    }
}
