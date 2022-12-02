import Foundation

struct ParseError : LocalizedError {
    let message: String

    var errorDescription: String? { message }
}

extension StringProtocol where Index == String.Index {
    func trimmed() -> String {
        self.trimmingCharacters(in: .whitespaces)
    }
}