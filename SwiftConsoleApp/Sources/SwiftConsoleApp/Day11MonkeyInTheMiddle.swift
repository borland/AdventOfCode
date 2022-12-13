import Foundation

private class Monkey {
    let operation: (_ old: Int) -> Int
    let throwTo: (_ x: Int) -> Int
    
    var items: [Int] // this can change
    var inspectionCount: Int = 0
    
    init(startingItems: [Int], operation: @escaping (Int)->Int, throwTo: @escaping (Int)->Int) {
        self.items = startingItems
        self.operation = operation
        self.throwTo = throwTo
    }
}

private let exampleMonkeys: [Monkey] = [
    Monkey(
        startingItems: [79, 98],
        operation: { old in old * 19 },
        throwTo: { $0 % 23 == 0 ? 2 : 3 }),
    Monkey(
        startingItems: [54, 65, 75, 74],
        operation: { old in old + 6 },
        throwTo: { $0 % 19 == 0 ? 2 : 0 }),
    Monkey(
        startingItems: [79, 60, 97],
        operation: { old in old * old },
        throwTo: { $0 % 13 == 0 ? 1 : 3 }),
    Monkey(
        startingItems: [74],
        operation: { old in old + 3 },
        throwTo: { $0 % 17 == 0 ? 0 : 1 }),
]

private let realMonkeys: [Monkey] = [
    Monkey(
        startingItems: [84, 66, 62, 69, 88, 91, 91],
        operation: { old in old * 11 },
        throwTo: { $0 % 2 == 0 ? 4 : 7 }),
    Monkey(
        startingItems: [98, 50, 76, 99],
        operation: { old in old * old },
        throwTo: { $0 % 7 == 0 ? 3 : 6 }),
    Monkey(
        startingItems: [72, 56, 94],
        operation: { old in old + 1 },
        throwTo: { $0 % 13 == 0 ? 4 : 0 }),
    Monkey(
        startingItems: [55, 88, 90, 77, 60, 67],
        operation: { old in old + 2 },
        throwTo: { $0 % 3 == 0 ? 6: 5 }),
    Monkey(
        startingItems: [69, 72, 63, 60, 72, 52, 63, 78],
        operation: { old in old * 13 },
        throwTo: { $0 % 19 == 0 ? 1: 7 }),
    Monkey(
        startingItems: [89, 73],
        operation: { old in old + 5 },
        throwTo: { $0 % 17 == 0 ? 2: 0 }),
    Monkey(
        startingItems: [78, 68, 98, 88, 66],
        operation: { old in old + 6 },
        throwTo: { $0 % 11 == 0 ? 2: 5 }),
    Monkey(
        startingItems: [70],
        operation: { old in old + 7 },
        throwTo: { $0 % 5 == 0 ? 1: 3 }),
]

struct Day11MonkeyInTheMiddleP1 {

    static func run() {
        let monkeys = exampleMonkeys
        
        for round in 0..<20 {
            print("===== round \(round) =============== ")
            // a round
            for monkey in monkeys {
                while !monkey.items.isEmpty {
                    let item = monkey.items.removeFirst()
                    monkey.inspectionCount += 1
                    
                    let worryLevel = monkey.operation(item) / 3
                    let throwTarget = monkey.throwTo(worryLevel)
                    monkeys[throwTarget].items.append(worryLevel)
                }
            }
            
            // print the output
            for (idx, monkey) in monkeys.enumerated() {
                let csv = monkey.items.map{ String($0) }.joined(separator: ",")
                print("Monkey \(idx): \(csv)")
            }
        }
        
        print("==================== ")
        let inspectionCounts = monkeys.map { $0.inspectionCount }
        for (idx, count) in inspectionCounts.enumerated() {
            print("Monkey \(idx) inspected items \(count) times.")
        }
        
        let topTwo = inspectionCounts.sorted(by: { $0 > $1 }).prefix(upTo: 2)
        print("topTwo = \(topTwo)")
        let grandTotal = topTwo[0] * topTwo[1]
        
        print("Monkey Business = \(grandTotal)")
    }
}

struct Puzzle11MonkeyInTheMiddleP2 {
    
    static func run() {
        let monkeys = exampleMonkeys
        
        for r in 0..<6 {
            print("round \(r)")
            
            for monkey in monkeys {
                while !monkey.items.isEmpty {
                    let item = monkey.items.removeFirst()
                    monkey.inspectionCount += 1
                    
                    let worryLevel = monkey.operation(item) // worry level no longer divides by 3.
                    let throwTarget = monkey.throwTo(worryLevel)
                    monkeys[throwTarget].items.append(worryLevel)
                }
            }
            
            // print the output
            for (idx, monkey) in monkeys.enumerated() {
                let csv = monkey.items.map{ String($0) }.joined(separator: ",")
                print("Monkey \(idx): \(csv)")
            }
        }
        
        print("==================== ")
        let inspectionCounts = monkeys.map { $0.inspectionCount }
        for (idx, count) in inspectionCounts.enumerated() {
            print("Monkey \(idx) inspected items \(count) times.")
        }
        
        let topTwo = inspectionCounts.sorted(by: { $0 > $1 }).prefix(upTo: 2)
        print("topTwo = \(topTwo)")
        let grandTotal = topTwo[0] * topTwo[1]
        
        print("Monkey Business = \(grandTotal)")
    }
}
