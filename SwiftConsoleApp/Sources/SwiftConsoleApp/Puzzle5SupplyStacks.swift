import Foundation

private class Stack {
    let number: Int
    var crates: [Character]
    
    init(number: Int) {
        self.number = number
        self.crates = []
    }
    
    // used to build the initial model where we load the text file in a top-down fashion
    func pushFront(_ crate: Character) {
        crates.insert(crate, at: 0)
    }
    
    // put a single crate on top of the stack
    func push(_ crate: Character) {
        crates.append(crate)
    }
    
    // remove and return a single crate from the top of the stack
    func pop() -> Character {
        crates.removeLast()
    }
    
    // put multiple crates on top of the stack (preserves order)
    func push(_ crates: [Character]) {
        self.crates.append(contentsOf: crates)
    }
    
    // remove multiple crates from the top of the stack (preserves order)
    func pop(numberOfCrates: Int) -> [Character] {
        if numberOfCrates > crates.count {
            fatalError("can't pop \(numberOfCrates) crates; our stack only has \(crates.count).")
        }
        let chars = Array(crates.suffix(numberOfCrates))
        crates.removeLast(numberOfCrates)
        return chars
    }
    
    // returns the crate character at the top of this stack, or an _ if the stack is empty
    func topCrate() -> Character {
        crates.last ?? "_"
    }
}

private struct Instruction : CustomStringConvertible {
    let numCratesToMove: Int
    let sourceStackNumber: Int
    let targetStackNumber: Int
    
    var description: String {
        "move \(numCratesToMove) from \(sourceStackNumber) to \(targetStackNumber)"
    }
}

fileprivate func parseFile(fileName: String) throws -> ([Stack],[Instruction]) {
    var stacks = [Stack]()
    var instructions = [Instruction]()

    enum ParseState {
        case initial
        case crateDef
        case instructions
    }
    
    var parseState: ParseState = .initial
    var numStacks = 0
    
    func pickupCrateDef(_ l: String) {
        var stackIdx = 0
        // I am so very glad I wrote inGroupsOf for the earlier exercise
        for str in l.inGroupsOf(4).map({ String($0).trimmed() }) {
            switch str.count {
            
            case 3 where str.first! == "[" && str.last! == "]": // it's a crate
                let crate = str[str.index(str.startIndex, offsetBy: 1)]
                stacks[stackIdx].pushFront(crate)
            
            case 0: // blank, no crate in this column
                break
                
            case 1 where str == "1": // it's the 1 2 3 row at the bottom of the stacks
                parseState = .instructions // don't need to worry about the blank line after 1 2 3 because we trim it away with includeEmpty
                return
            
            default: fatalError("unhandled crate def \(str)")
            }
            stackIdx += 1
        }
    }
    
    func pickupInstruction(_ l: String) {
        let components = l.split(separator: " ")
        guard components.count == 6,
              let numCratesToMove = Int(components[1]),
              let sourceStackNumber = Int(components[3]),
              let targetStackNumber = Int(components[5]) else {
            fatalError("Can't pickup instruction from \(l)")
        }
        instructions.append(Instruction(numCratesToMove: numCratesToMove, sourceStackNumber: sourceStackNumber, targetStackNumber: targetStackNumber))
    }
    
    for line in try linesInFile(fileName, includeEmpty: false, trim: false) {
        switch parseState {
        case .initial:
            // work out how many stacks we have. A line looks like this:
            // [Z] [M] [P]
            // we know that each item is only a single letter, so that means 3 characters per crate plus a space separator.
            // There is no trailing space, so the number of stacks = (lineLength +1) / 4
            if (line.count + 1) % 4 != 0 {
                fatalError("line \(line) had \(line.count) characters which was not a valid length for the stack definition")
            }
            numStacks = (line.count + 1) / 4
            print("determine \(numStacks) stacks in this input")
            
            stacks = (0..<numStacks).map { i in Stack(number: i + 1) }
            
            parseState = .crateDef
            pickupCrateDef(line)
            
            // phase 2: load all the crates into the stacks
        case .crateDef:
            pickupCrateDef(line)
            
        case .instructions:
            pickupInstruction(line)
        }
    }
    
    printStacks(stacks)
    
    return (stacks, instructions)
}

// so I can make sense of this thing
private func printStacks(_ stacks: [Stack]) {
    func crateString(_ crates: [Character]) -> String {
        return crates.map { c in "[\(c)]" }.joined(separator: " ")
    }
    
    for stack in stacks {
        print("Stack \(stack.number): \(crateString(stack.crates))")
    }
}

struct Puzzle5SupplyStacksP1 {
    
    public static func run(fileName: String) throws {
        let (stacks, instructions) = try parseFile(fileName: fileName)
        
        for instruction in instructions {
            let sourceStack = stacks[instruction.sourceStackNumber-1]
            let targetStack = stacks[instruction.targetStackNumber-1]
            
            for _ in 0..<instruction.numCratesToMove {
                targetStack.push(sourceStack.pop())
            }
            
//            print("====================")
//            print(instruction)
//            printStacks(stacks)
        }
        
        print("Crates at top of each stack = \(String(stacks.map{ $0.topCrate() }))")
    }
}

struct Puzzle5SupplyStacksP2 {
    
    public static func run(fileName: String) throws {
        let (stacks, instructions) = try parseFile(fileName: fileName)
        
        for instruction in instructions {
            let sourceStack = stacks[instruction.sourceStackNumber-1]
            let targetStack = stacks[instruction.targetStackNumber-1]
            
            targetStack.push(sourceStack.pop(numberOfCrates: instruction.numCratesToMove))
            
//            print("====================")
//            print(instruction)
//            printStacks(stacks)
        }
        
        print("Crates at top of each stack = \(String(stacks.map{ $0.topCrate() }))")
    }
}
