import Foundation

struct ParseError : LocalizedError {
    let message: String

    var errorDescription: String? { message }
}

extension StringProtocol {
    func trimmed() -> String {
        self.trimmingCharacters(in: .whitespaces)
    }
}

func linesInFile(_ fileName: String, includeEmpty: Bool = false, trim: Bool = true) throws -> [String] {
    let contents = try String(contentsOfFile: fileName, encoding: .utf8)
    let lines = contents
        .split(separator: "\n", omittingEmptySubsequences: !includeEmpty)
    
    let trimmedLines:[String] = trim ?
        lines.map { line in line.trimmed() } :
        lines.map { line in String(line) }
    
    return includeEmpty ?
        trimmedLines :
        trimmedLines.filter { line in line != "" }
}

extension Sequence {
    func sum() -> Element where Element : AdditiveArithmetic {
        return self.reduce(.zero, +)
    }
    
    func sum<T>(_ of: (Element) -> T) -> T where T : AdditiveArithmetic {
        return self.reduce(.zero) { memo, element in
            memo + of(element)
        }
    }
    
    func ofType<T>() -> [T] {
        return self.compactMap {
            $0 as? T
        }
    }
    
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
