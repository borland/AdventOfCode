import Foundation

do {
    guard let inputFile = CommandLine.arguments.last else {
        fatalError("Please specify the input file on the command line")
    }
    try Puzzle1CalorieCounting.part2(inputFile: inputFile)

} catch let err {
    print("Failed with error \(err)")
}