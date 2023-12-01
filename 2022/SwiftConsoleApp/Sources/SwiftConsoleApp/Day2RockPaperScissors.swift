enum Shape {
    case rock, paper, scissors

    func score() -> Int {
        switch self {
            case .rock: return 1
            case .paper: return 2
            case .scissors: return 3
        }
    }
}

enum Outcome {
    case lose, draw, win

    func score() -> Int {
        switch self {
            case .lose: return 0
            case .draw: return 3
            case .win: return 6
        }
    }
}

struct RoundChoices {
    let opponentChoice: Shape
    let yourChoice: Shape

    func play() -> Outcome {
        switch (self.opponentChoice, self.yourChoice) {
            case (.rock, .paper), (.paper, .scissors), (.scissors, .rock): return .win
            case (.rock, .rock), (.paper, .paper), (.scissors, .scissors): return .draw
            case (.rock, .scissors), (.paper, .rock), (.scissors, .paper): return .lose
        }
    }
}

struct Day2RockPaperScissorsP1 {

    static func parseFile(fileName: String) throws -> [RoundChoices] {
        return try linesInFile(fileName)
            .map { line in
                let tokens = line.split(separator: " ", maxSplits: 2)
                guard tokens.count == 2 else { throw ParseError(message: "Unparseable line \(line)") }

                let opponentToken = tokens[0].uppercased()
                let yourToken = tokens[1].uppercased()

                let opponentChoice: Shape
                switch opponentToken {
                    case "A": opponentChoice = .rock
                    case "B": opponentChoice = .paper
                    case "C": opponentChoice = .scissors
                    default: throw ParseError(message: "Unparseable opponent choice \(opponentToken)")
                }
                let yourChoice: Shape
                switch yourToken {
                    case "X": yourChoice = .rock
                    case "Y": yourChoice = .paper
                    case "Z": yourChoice = .scissors
                    default: throw ParseError(message: "Unparseable your choice \(yourToken)")
                }

                return RoundChoices(opponentChoice: opponentChoice, yourChoice: yourChoice)
            }
    }

    public static func run(fileName: String) throws {
        let grandTotal = try parseFile(fileName: fileName)
            .map { entry in 
                let shapeScore = entry.yourChoice.score()
                let outcomeScore = entry.play().score()
                let totalScore = shapeScore + outcomeScore
                print(
                    "Game: Opponent \(entry.opponentChoice); You \(entry.yourChoice). ShapeScore=\(shapeScore), OutcomeScore=\(outcomeScore), Total=\(totalScore)")

                return totalScore
            }
            .reduce(0, +)

        print("Grand Total=\(grandTotal)")
    }
}

struct Day2RockPaperScissorsP2 {
    struct GuideEntry {
        let opponentChoice: Shape
        let desiredOutcome: Outcome
    }

    static func parseFile(fileName: String) throws -> [GuideEntry] {
        return try linesInFile(fileName)
            .map { line in
                let tokens = line.split(separator: " ", maxSplits: 2)
                guard tokens.count == 2 else { throw ParseError(message: "Unparseable line \(line)") }

                let opponentToken = tokens[0].uppercased()
                let outcomeToken = tokens[1].uppercased()

                let opponentChoice: Shape
                switch opponentToken {
                    case "A": opponentChoice = .rock
                    case "B": opponentChoice = .paper
                    case "C": opponentChoice = .scissors
                    default: throw ParseError(message: "Unparseable opponentToken \(opponentToken)")
                }
                let outcome: Outcome
                switch outcomeToken {
                    case "X": outcome = .lose
                    case "Y": outcome = .draw
                    case "Z": outcome = .win
                    default: throw ParseError(message: "Unparseable outcomeToken \(outcomeToken)")
                }

                return GuideEntry(opponentChoice: opponentChoice, desiredOutcome: outcome)
            }
    }

    public static func run(fileName: String) throws
    {
        let grandTotal = try parseFile(fileName: fileName)
            .map { entry in 
                let yourChoice: Shape
                switch (entry.opponentChoice, entry.desiredOutcome) {
                    // If the opponent chooses Rock, we want to Win, so we choose Paper
                    case (.rock, .win): yourChoice = .paper
                    case (.rock, .draw): yourChoice = .rock
                    case (.rock, .lose): yourChoice = .scissors

                    case (.paper, .win): yourChoice = .scissors
                    case (.paper, .draw): yourChoice = .paper
                    case (.paper, .lose): yourChoice = .rock

                    case (.scissors, .win): yourChoice = .rock
                    case (.scissors, .draw): yourChoice = .scissors
                    case (.scissors, .lose): yourChoice = .paper
                };

                let totalScore = yourChoice.score() + entry.desiredOutcome.score()
                print(
                    "Game: Opponent \(entry.opponentChoice); Desired \(entry.desiredOutcome); You \(yourChoice). Total=\(totalScore)")

                return totalScore
            }
            .reduce(0, +)

        print("Grand Total=\(grandTotal)")
    }
}

