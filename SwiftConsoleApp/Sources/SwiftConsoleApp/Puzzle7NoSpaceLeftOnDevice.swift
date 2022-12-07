import Foundation

struct VirtualFileSystem
{
    class Entry {
        var parent: Dir? // everyone has a parent except the root
        var name: String // everyone has a name
        
        init(parent: Dir?, name: String) {
            self.parent = parent
            self.name = name
        }
    }
    
    class Dir : Entry {
        var entries: [Entry]
        
        init(parent: Dir?, name: String, entries: [Entry] = []) {
            self.entries = entries
            super.init(parent: parent, name: name)
        }
    }

    class File : Entry {
        var size: Int
        
        init(parent: Dir?, name: String, size: Int) {
            self.size = size
            super.init(parent: parent, name: name)
        }
    }
}

extension VirtualFileSystem.Dir {
    func printRecursive() -> String {
        var output = ""
        
        func pr(indent: Int, entry: VirtualFileSystem.Entry) {
            let indentStr = String(repeating: " ", count: indent)
            switch entry {
            case let d as VirtualFileSystem.Dir:
                output += "\(indentStr)- \(d.name) (dir)\n"
                for child in d.entries {
                    pr(indent: indent + 2, entry: child)
                }
            case let f as VirtualFileSystem.File:
                output += "\(indentStr)- \(f.name) (file, size=\(f.size))\n"
            default:
                fatalError("Unhandled entry \(entry)")
            }
        }
        pr(indent: 0, entry: self)
        return output
    }
}

enum TermState {
    case interpret
    case parseLsOutput
}

fileprivate func parseTerminalLog(lines: [String]) -> VirtualFileSystem.Dir {
    let root = VirtualFileSystem.Dir(parent: nil, name: "/")
    var workingDir = root
    
    // interpreter!
    var state: TermState = .interpret
    
    func interpretLine(_ components: [Substring]) {
        if components.count == 3 && components[0] == "$" && components[1] == "cd" {
            let changeDirTarget = components[2]
            if changeDirTarget == "/" {
                workingDir = root
            } else if changeDirTarget == "..", let p = workingDir.parent { // go up if possible
                workingDir = p
            } else {
                let c = workingDir
                    .entries
                    .compactMap { $0 as? VirtualFileSystem.Dir }
                    .first { $0.name == changeDirTarget }
                
                if let c {
                    workingDir = c
                } else {
                    fatalError("can't cd into '\(changeDirTarget)', it was not found in \(workingDir.name)")
                }
            }
        } else if components.count == 2 && components[0] == "$" && components[1] == "ls" {
            state = .parseLsOutput
        } else {
            fatalError("can't interpret \(components)")
        }
    }
    
    func pickupDirOutput(_ components: [Substring]) {
        guard components.count == 2 else { fatalError("can't pickup dir output \(components)") }
        
        if components[0] == "dir" {
            // we've seen that a dir exists, add it to our working directory
            workingDir.entries.append(VirtualFileSystem.Dir(parent: workingDir, name: String(components[1])))
        } else if let sz = Int(components[0]) {
            workingDir.entries.append(VirtualFileSystem.File(parent: workingDir, name: String(components[1]), size: sz))
        } else {
            fatalError("can't pickup dir output \(components)")
        }
    }
    
    for line in lines {
        let components = line.split(separator: " ")
        guard components.count >= 2 else { fatalError("line \(line) didn't have at least two components") }
        switch state
        {
        case .interpret:
            interpretLine(components)
            
        case .parseLsOutput:
            if components[0] == "$" {
                state = .interpret
                interpretLine(components)
            } else {
                pickupDirOutput(components)
            }
        }
    }
    return root
}

struct Puzzle7NoSpaceLeftOnDeviceP1 {

    public static func run(fileName: String) throws {
        let lines = try linesInFile(fileName)
        let rootDir = parseTerminalLog(lines: lines)
        print(rootDir.printRecursive())
        
        // find all of the directories with a total size of at most 100000 and add them all up
        // depth-first walk of the tree
        
        var sum = 0
        
        func getSize(entry: VirtualFileSystem.Entry) -> Int {
            switch entry {
            case let d as VirtualFileSystem.Dir:
                var size = 0
                for child in d.entries {
                    size += getSize(entry: child)
                }
                if size <= 100000 {
                    print("directory \(d.name) qualifies, its size is \(size)")
                    sum += size
                }
                return size
            case let f as VirtualFileSystem.File:
                return f.size
            default:
                fatalError("Unhandled entry \(entry)")
            }
        }
        let rootTotalSize = getSize(entry: rootDir)
        
        print("rootTotalSize = \(rootTotalSize); weird sum = \(sum)")
    }
}

struct Puzzle7NoSpaceLeftOnDeviceP2 {

    public static func run(fileName: String) throws {
        let lines = try linesInFile(fileName)
        let rootDir = parseTerminalLog(lines: lines)
        print(rootDir.printRecursive())

        // every time we calculate the size of a dir, stash it here
        // so later we can find the smallest dir over our threshold
        var dirSizes:[(dir: VirtualFileSystem.Dir, size: Int)] = []
        
        func getSize(entry: VirtualFileSystem.Entry) -> Int {
            switch entry {
            case let d as VirtualFileSystem.Dir:
                var size = 0
                for child in d.entries {
                    size += getSize(entry: child)
                }
                dirSizes.append((d, size))
                return size
            case let f as VirtualFileSystem.File:
                return f.size
            default:
                fatalError("Unhandled entry \(entry)")
            }
        }
        let rootTotalSize = getSize(entry: rootDir)
        let diskSize = 70000000
        let desiredFreeSpace = 30000000
        let freeSpace = diskSize - rootTotalSize
        
        let neededFreeSpace = desiredFreeSpace - freeSpace
        
        print("rootTotalSize = \(rootTotalSize); neededFreeSpace = \(neededFreeSpace)")
        
        let b = dirSizes
            .sorted { (a, b) in a.size < b.size }
            .map { a in print("dir \(a.dir.name), size=\(a.size)"); return a }
            .first { a in a.size >= neededFreeSpace }
        
        if let b {
            print("Our deletion candidate is \(b.dir.name) with size \(b.size)")
        }
    }
}
