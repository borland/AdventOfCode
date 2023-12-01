import Foundation


private class Monkey {
    let name: String
    let action: () -> Int?
    
    var yelled: Int? = nil // nil if they haven't yelled yet
    
    init(name: String, action: @escaping () -> Int?) {
        self.name = name
        self.action = action
    }
}

private func parse(lines: [String]) -> [Monkey] {
    var lookup: [String:Monkey] = [:]
    
    return lines.map { line in
        let outerComponents = line.split(separator: ":").map{ $0.trimmed() }
        
        let name = outerComponents[0]
        let action: () -> Int?
        
        let exprComponents = outerComponents[1].split(separator: " ").map { $0.trimmed() }
        if exprComponents.count == 1, let val = Int(exprComponents[0]) {
            action = { val }
        } else if exprComponents.count == 3 {
            let leftMonkey = exprComponents[0]
            let rightMonkey = exprComponents[2]
            let op: (Int, Int) -> Int
            switch exprComponents[1] {
            case "+": op = { $0 + $1 }
            case "-": op = { $0 - $1 }
            case "*": op = { $0 * $1 }
            case "/": op = { $0 / $1 }
            default: fatalError("can't parse line \(line); invalid op \(exprComponents[1])")
            }
            
            action = {
                if let leftVal = lookup[leftMonkey]?.yelled,
                   let rightVal = lookup[rightMonkey]?.yelled {
                    // both our inputs have yelled, we can too.
                    return op(leftVal, rightVal)
                }
                return nil
            }
        } else {
            fatalError("can't parse line \(line)")
        }
        
        let monkey = Monkey(name: name, action: action)
        lookup[name] = monkey
        return monkey
    }
}


struct Day21MonkeyMathP1 {
    static func run(fileName: String) throws {
        let monkeys = parse(lines: try linesInFile(fileName))
        
        for i in 0..<100 {
            print("round \(i)")
            for m in monkeys {
                if m.yelled == nil {
                    m.yelled = m.action()
                    if let y = m.yelled {
                        if m.name == "root" {
                            print("\(m.name) has yelled the value \(y)")
                            return // all done
                        }
                    }
                }
            }
        }
    }
}
