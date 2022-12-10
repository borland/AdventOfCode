import Foundation

private enum Instruction {
    case noop
    case addx(Int)
}

private class Cpu {
    var cycleCounter = 0
    var registerX = 1
    
    var observer: ((_ cycle: Int, _ x: Int) -> ())?
    
    func execute(instruction: Instruction) {
        cycleCounter += 1
        switch instruction {
        case .noop:
            observer?(cycleCounter, registerX)
        case .addx(let operand):
            observer?(cycleCounter, registerX)
            cycleCounter += 1
            observer?(cycleCounter, registerX)
            registerX += operand
        }
    }
}

private func parseAndExecute(lines: [String], cpu: Cpu) {
    for l in lines {
        let tokens = l.split(separator: " ", maxSplits: 2)
        if tokens.count == 1 && tokens[0] == "noop" {
            cpu.execute(instruction: .noop)
        } else if tokens.count == 2 && tokens[0] == "addx", let operand = Int(tokens[1]) {
            cpu.execute(instruction: .addx(operand))
        }
    }
}

struct Puzzle10CathodeRayTubeP1 {
    
    public static func run(fileName: String) throws {
        var totalSignalStrength = 0

        let cpu = Cpu()
        cpu.observer = { cycle, x in
            switch cycle {
            case 20, 60, 100, 140, 180, 220:
                print("at cycle \(cycle), x is \(x), signal strength is \(cycle * x)")
                totalSignalStrength += cycle * x
            default: break
            }
        }
        
        parseAndExecute(lines: try linesInFile(fileName), cpu: cpu)
        print("final total signal strength is \(totalSignalStrength)")
    }
}

private class Crt {
    let rows = 6
    let cols = 40
    var pixelBuffer: [Bool]
    
    init() {
        self.pixelBuffer = Array(repeating: false, count: rows * cols)
    }
    
    subscript(x: Int, y: Int) -> Bool {
        get {
            if x < 0 || y < 0 || x >= cols || y >= rows { fatalError("x or y out of range") }
            return pixelBuffer[y * cols + x]
        }
        set {
            if x < 0 || y < 0 || x >= cols || y >= rows { fatalError("x or y out of range") }
            pixelBuffer[y * cols + x] = newValue
        }
    }
    
    func render() -> String {
        var output = ""
        for i in pixelBuffer.indices {
            output += pixelBuffer[i] ? "#" : "."
            if i % cols == cols-1 {
                output += "\n"
            }
        }
        return output
    }
}

struct Puzzle10CathodeRayTubeP2 {
    
    public static func run(fileName: String) throws {
        let cpu = Cpu()
        let crt = Crt()
        
        cpu.observer = { cycle, xRegister in
            // our CPU cycle numbers are 1-based, need to offset this for the maths to work
            let xPos = (cycle-1) % crt.cols
            let yPos = (cycle-1) / crt.cols

            // if the value of xRegister matches our xPos (plus or minus 1), then draw the pixel
            if (xRegister-1...xRegister+1).contains(xPos) {
                crt[xPos, yPos] = true
            }
        }
        
        parseAndExecute(lines: try linesInFile(fileName), cpu: cpu)
        print(crt.render())
    }
}

