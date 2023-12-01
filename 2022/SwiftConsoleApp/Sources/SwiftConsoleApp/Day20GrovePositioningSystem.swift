import Foundation

private func shift<T>(_ array: inout [T], index: Int, by: Int) {
    let item = array.remove(at: index)
    var insertionIdx = (index + by) % array.count
    if insertionIdx <= 0 {
        insertionIdx += array.count
    }
    array.insert(item, at: insertionIdx)
}

struct Day20GrovePositioningSystemP1 {
    private static func mix(_ numbers: [Int]) -> [Int] {
        typealias ListItem = (number: Int, originalIndex: Int)
        
        var listItems = numbers.enumerated().map { (idx, n) in ListItem(number: n, originalIndex: idx) }
        
        for i in listItems.indices {
            guard let pos = listItems.firstIndex(where: { $0.originalIndex == i }) else {
                fatalError("logic error, can't find an item with originalIndex \(i)")
            }
    //        print("\(result[pos].number) moves from index \(pos) by \(result[pos].number)")
            shift(&listItems, index: pos, by: listItems[pos].number)
    //        print(result)
        }
        
        return listItems.map { $0.number }
    }
    
    static func run(fileName: String) throws {
        let numbers = try linesInFile(fileName).compactMap { Int($0) }
        print(numbers)
        
        let mixed = mix(numbers)
        
        print(mixed)
        guard let offsetOfZeroValue = mixed.firstIndex(of: 0) else { fatalError("can't find zero value") }
        print("offsetOfZeroValue = \(offsetOfZeroValue)")
        
        var sum = 0
        for x in [1000, 2000, 3000] {
            let target = mixed[(x + offsetOfZeroValue) % mixed.count]
            print(target)
            sum += target
        }
        print("sum of targets = \(sum)")
    }
}

struct Day20GrovePositioningSystemP2 {
    typealias ListItem = (number: Int, originalIndex: Int)
    
    private static func mix(_ listItems: [ListItem]) -> [ListItem] {
        var result = listItems
        for i in result.indices {
            guard let pos = result.firstIndex(where: { $0.originalIndex == i }) else {
                fatalError("logic error, can't find an item with originalIndex \(i)")
            }
            shift(&result, index: pos, by: result[pos].number)
        }
        
        return result
    }
    
    static func run(fileName: String) throws {
        let decryptionKey = 811589153
        
        let numbers = try linesInFile(fileName).compactMap { Int($0) }
        var listItems = numbers.enumerated().map { (idx, n) in
            ListItem(number: n * decryptionKey, originalIndex: idx)
        }
        
        for _ in 0..<10 {
            listItems = mix(listItems)
//            print("after \(i+1) round of mixing:")
//            print(listItems)
        }
        
        guard let offsetOfZeroValue = listItems.firstIndex(where: { $0.number == 0 }) else { fatalError("can't find zero value") }
//        print("offsetOfZeroValue = \(offsetOfZeroValue)")
        
        var sum = 0
        for x in [1000, 2000, 3000] {
            let target = listItems[(x + offsetOfZeroValue) % listItems.count]
            print(target)
            sum += target.number
        }
        print("sum of targets = \(sum)")
    }
}
