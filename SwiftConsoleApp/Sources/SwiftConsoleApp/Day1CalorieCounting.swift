import Foundation

struct ElfInfo {
    let elfNumber: Int
    let calories: Int
}

struct Day1CalorieCounting {

    static func part1(inputFile: String) throws {
        var elfNum = 1
        var currentElfCalories = 0
        var lastLineWasBlank = false

        var maxElf = ElfInfo(elfNumber: 0, calories: 0)

        func recordCurrent() {
            print("Elf \(elfNum) is carrying \(currentElfCalories) calories")
            if currentElfCalories > maxElf.calories {
                maxElf = ElfInfo(elfNumber: elfNum, calories: currentElfCalories)
            }

            elfNum += 1
            currentElfCalories = 0
        }

        let contents = try String(contentsOfFile: inputFile, encoding: .utf8)
        for line in contents.split(separator: "\n", omittingEmptySubsequences: false).map({ $0.trimmed() }) {
            if line == "" && !lastLineWasBlank {
                lastLineWasBlank = true

                recordCurrent()
                continue;
            }
            
            if let calories = Int(line) {
                lastLineWasBlank = false
                currentElfCalories += calories
            } else {
                throw ParseError(message: "Can't deal with line \(line)")
            }
        }
        recordCurrent()

        print("The elf carrying the most calories is \(maxElf.elfNumber) with \(maxElf.calories) calories")
    }

    static func part2(inputFile: String) throws {
        // keep the state machine to enumerate the file, but collect results into a list instead of just keeping maxElf
        var elfNum = 1
        var currentElfCalories = 0
        var lastLineWasBlank = false

        var allElves = [ElfInfo]()
        allElves.reserveCapacity(300)

        func recordCurrent() {
            print("Elf \(elfNum) is carrying \(currentElfCalories) calories")
            allElves.append(ElfInfo(elfNumber: elfNum, calories: currentElfCalories))

            elfNum += 1
            currentElfCalories = 0
        }

        let contents = try String(contentsOfFile: inputFile, encoding: .utf8)
        for line in contents.split(separator: "\n", omittingEmptySubsequences: false).map({ $0.trimmed() }) {
            if line == "" && !lastLineWasBlank {
                lastLineWasBlank = true

                recordCurrent()
                continue;
            }
            
            if let calories = Int(line) {
                lastLineWasBlank = false
                currentElfCalories += calories
            } else {
                throw ParseError(message: "Can't deal with line \(line)")
            }
        }
        recordCurrent()

        let top3Totalcalories = allElves.sorted { $0.calories > $1.calories }.prefix(3).reduce(0) { $0 + $1.calories }

        print("The top 3 elves together carry \(top3Totalcalories) calories")
    }
}

// ParseError and Trimmed extensions defined in main.swift
