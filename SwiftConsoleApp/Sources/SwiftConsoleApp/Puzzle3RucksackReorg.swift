import Foundation

struct Rucksack {
    let compartment1: String
    let compartment2: String
}

struct Puzzle3RucksackReorgP1 {

    static func parseFile(fileName: String) throws -> [Rucksack] {
        let contents = try String(contentsOfFile: fileName, encoding: .utf8)
        return contents
            .split(separator: "\n", omittingEmptySubsequences: true)
            .map { line in line.trimmed() }
            .filter { line in line != "" }
            .map { line in
                let lineLen = line.count
                if lineLen % 2 != 0 {
                    fatalError("Can't deal with uneven line \(line)")
                }
                let pivot = line.index(line.startIndex, offsetBy: lineLen / 2)
                let comp1 = String(line[line.startIndex..<pivot])
                let comp2 = String(line[pivot..<line.endIndex])

                return Rucksack(compartment1: comp1, compartment2: comp2)
            }
    }
    
    static func priority(character: Character) -> Int {
        let asciiValues = "azAZ".map { $0.asciiValue! }
        let a = asciiValues[0]
        let z = asciiValues[1]
        let A = asciiValues[2]
        let Z = asciiValues[3]
        
        let i = character.asciiValue!
        switch i {
        case a ... z:
            return Int(i - a) + 1
        case A ... Z:
            return Int(i - A) + 27
        default:
            fatalError("Unexpected \(character)")
        }
    }

    public static func run(fileName: String) throws {
        let grandTotal = try parseFile(fileName: fileName)
            .map { entry in
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
            .reduce(0, +)

        print("Grand Total=\(grandTotal)")
    }
}
