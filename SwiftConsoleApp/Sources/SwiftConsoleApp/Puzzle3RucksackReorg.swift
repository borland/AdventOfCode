import Foundation

private struct Rucksack {
    let contents: String
    let compartment1: Substring
    let compartment2: Substring
}

private let asciiValues = "azAZ".map { $0.asciiValue! }
private let a = asciiValues[0]
private let z = asciiValues[1]
private let A = asciiValues[2]
private let Z = asciiValues[3]

private func priority(character: Character) -> Int {
    let char = character.asciiValue!
    switch char {
    case a ... z:
        return Int(char - a) + 1
    case A ... Z:
        return Int(char - A) + 27
    default:
        fatalError("Unexpected \(character)")
    }
}

struct Puzzle3RucksackReorgP1 {
    fileprivate static func parseFile(fileName: String) throws -> [Rucksack] {
        return try linesInFile(fileName)
            .map { line in
                let lineLen = line.count
                if lineLen % 2 != 0 {
                    fatalError("Can't deal with uneven line \(line)")
                }
                let pivot = line.index(line.startIndex, offsetBy: lineLen / 2)
                let comp1 = line[line.startIndex..<pivot]
                let comp2 = line[pivot..<line.endIndex]
                
                return Rucksack(contents: line, compartment1: comp1, compartment2: comp2)
            }
    }
    
    public static func run(fileName: String) throws {
        let grandTotal = try parseFile(fileName: fileName)
            .sum { entry in
                var common = Set<Character>()
                for c in entry.compartment1 {
                    if entry.compartment2.firstIndex(of: c) != nil {
                        common.insert(c)
                    }
                }
                
                let priorities = common.map { c in priority(character: c) }.reduce(0, +)
                //                print("Ruckack: comp1=\(entry.compartment1) comp2=\(entry.compartment2) commonItems=\(common) priorities=\(priorities)")
                return priorities
            }
        
        print("Grand Total=\(grandTotal)")
    }
}

struct Puzzle3RucksackReorgP2 {
    private static func parseFile(fileName: String) throws -> [[Rucksack]] {
        return try Puzzle3RucksackReorgP1
            .parseFile(fileName: fileName)
            .inGroupsOf(3)
    }
    
    public static func run(fileName: String) throws {
        let grandTotal = try parseFile(fileName: fileName)
            .sum { entrySet in
                var common = [Character:Int]()
                for rucksack in entrySet {
                    for c in rucksack.contents.distinct() {
                        common[c] = (common[c] ?? 0) + 1
                    }
                }
                if let kv = common.first(where: { (k, v) in v == entrySet.count }) {
//                    print("group: character=\(kv.key)")
                    return priority(character: kv.key)
                } else {
                    print("group: no common character!")
                    return 0 // no common item
                }
            }
        
        print("Grand Total=\(grandTotal)")
    }
}


