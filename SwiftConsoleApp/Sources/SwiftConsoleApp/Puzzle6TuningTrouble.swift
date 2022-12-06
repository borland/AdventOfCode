import Foundation

private func append(_ char: Character, onto: inout [Character], max: Int) {
    if onto.count < max {
        onto.append(char)
    } else { // actually shift onto the end
        for i in 1..<onto.count {
            onto[i-1] = onto[i]
        }
        onto[onto.count-1] = char
    }
}


private func allDifferent(in array: [Character]) -> Bool {
    let s = Set<Character>(array) // Set removes dupes
    return s.count == array.count
}

private func findUniqueMarker(lines: [String], uniqueCharCount: Int) {
    for line in lines {
        var idx = 0
        var last: [Character] = []
        
        for char in line {
            append(char, onto: &last, max: uniqueCharCount)
            idx += 1
            
            if last.count == uniqueCharCount && allDifferent(in: last) {
                print("\(line): first marker after character \(idx)")
                break
            }
        }
    }
}

struct Puzzle6TuningTroubleP1 {
    public static func run(fileName: String) throws {
        let lines = try linesInFile(fileName)
        findUniqueMarker(lines: lines, uniqueCharCount: 4)
    }
}

struct Puzzle6TuningTroubleP2 {
    public static func run(fileName: String) throws {
        let lines = try linesInFile(fileName)
        findUniqueMarker(lines: lines, uniqueCharCount: 14)
    }
}
