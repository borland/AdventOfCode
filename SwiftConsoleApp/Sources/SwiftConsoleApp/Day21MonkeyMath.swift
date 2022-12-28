import Foundation


private class Monkey {
    let name: String
    let action: ([Monkey]) -> Int?
    
    var yelled: Int? = nil // nil if they haven't yelled yet
    
    init(name: String, action: @escaping ([Monkey]) -> Int?) {
        self.name = name
        self.action = action
    }
}

private func parse(lines: [String]) -> [Monkey] {
    return lines.map { line in
        let outerComponents = line.split(separator: ":").map{ $0.trimmed() }
        
        let name = outerComponents[0]
        let action: ([Monkey]) -> Int?
        
        let exprComponents = outerComponents[1].split(separator: " ").map { $0.trimmed() }
        if exprComponents.count == 1, let val = Int(exprComponents[0]) {
            action = { _ in val }
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
            
            action = { (monkeys) in
                if let leftVal = monkeys.first(where: { $0.name == leftMonkey })?.yelled,
                   let rightVal = monkeys.first(where: { $0.name == rightMonkey })?.yelled {
                    // both our inputs have yelled, we can too.
                    return op(leftVal, rightVal)
                }
                return nil
            }
        } else {
            fatalError("can't parse line \(line)")
        }
        
        return Monkey(name: name, action: action)
    }
}


struct Day21MonkeyMathP1 {
    static func run(fileName: String) throws {
        let monkeys = parse(lines: try linesInFile(fileName))
        
        for i in 0..<100 {
            print("round \(i)")
            for m in monkeys {
                if m.yelled == nil {
                    m.yelled = m.action(monkeys)
                    if let y = m.yelled {
                        print("\(m.name) has yelled the value \(y)")
                        if m.name == "root" {
                            return // all done
                        }
                    } else {
                        print("\(m.name) has not yet yelled anything")
                    }
                }
            }
        }
    }
}
