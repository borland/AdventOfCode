using System.Diagnostics;

namespace aoc203;

class Day7
{
    public static readonly string ExampleInput = """
        32T3K 765
        T55J5 684
        KK677 28
        KTJJT 220
        QQQJA 483
        """;

    public static string LoadInput(InputSource inputSource) => inputSource switch
    {
        InputSource.Example => ExampleInput,
        InputSource.Real => File.ReadAllText("Day7-input.txt"),
        _ => throw new ArgumentException($"Can't handle inputSource {inputSource}")
    };


    public static void Part1(InputSource inputSource)
    {
        var hands = ParseHands(LoadInput(inputSource))
            .Select(x => new Hand(DetermineHandTypeAlt(x.cards), x.cards, x.bid))
            .ToArray();

        var rankedHands = OrderAndAssignRanks(hands, new HandCardTypeComparer(RegularCardDefinitions));
        var totalWinnings = 0;
        foreach (var r in rankedHands)
        {
            var winnings = r.Rank * r.Bid;
            totalWinnings += winnings;

            // Console.WriteLine($"Rank {r.Rank} - Hand {r.Cards} which is {r.HandType} with bid {r.Bid}");
        }

        // Console.WriteLine($"Total winnings = {totalWinnings}");
    }

    public static void Part2(InputSource inputSource)
    {
        var hands = ParseHands(LoadInput(inputSource))
            .Select(x => new Hand(DetermineHandTypeUsingJokers(x.cards), x.cards, x.bid))
            .ToArray();

        var rankedHands = OrderAndAssignRanks(hands, new HandCardTypeComparer(CardDefinitionsWithJoker));
        var totalWinnings = 0;
        foreach (var r in rankedHands)
        {
            var winnings = r.Rank * r.Bid;
            totalWinnings += winnings;

            Console.WriteLine($"Rank {r.Rank} - Hand {r.Cards} which is {r.HandType} with bid {r.Bid}");
        }

        Console.WriteLine($"Total winnings = {totalWinnings}");
    }

    // The index in this string controls which card is stronger. Ace is strongest, 2 is weakest.
    static readonly string RegularCardDefinitions = "23456789TJQKA";
    // Joker is the weakest by face-value alone
    static readonly string CardDefinitionsWithJoker = "J23456789TQKA";

    static bool IsValidCard(char ch) => RegularCardDefinitions.Contains(ch);

    static IEnumerable<(string cards, int bid)> ParseHands(string input)
    {
        foreach (var line in input.EnumerateLines())
        {
            var reader = new DelimitedLineReader(line);
            reader.Scan(IsValidCard, out var cards).MovePast(" ").ReadInt(out var bid);
            yield return (cards.ToString(), bid);
        }
    }

    record RankedHand(int Rank, HandType HandType, string Cards, int Bid);

    static IEnumerable<RankedHand> OrderAndAssignRanks(IEnumerable<Hand> hands, HandCardTypeComparer comparer)
    {
        Hand? prevHand = null;
        int prevRank = 1;

        foreach (var h in hands.Order(comparer))
        {
            int rank = (prevHand == null)
                ? 1 // we know the list is sorted so the first thing is always rank 1
                : (comparer.Compare(h, prevHand) == 0)
                    ? rank = prevRank // identical hands get the same rank
                    : rank = ++prevRank; // else it's whatever the last rank was +1

            prevHand = h;
            yield return new RankedHand(rank, h.HandType, h.Cards, h.Bid);
        }
    }

    class HandCardTypeComparer(string cardDefinitions) : IComparer<Hand>
    {
        // allows us to alter the order of importance depending on whether jokers are in play
        private readonly string cardDefinitions = cardDefinitions;

        public int Compare(Hand? x, Hand? y)
        {
            if (x == null || y == null) throw new NotSupportedException("can't compare null hands");

            var typeCompareResult = x.HandType.CompareTo(y.HandType);
            if (typeCompareResult != 0) return typeCompareResult;

            return new CardStringValueComparer(cardDefinitions).Compare(x.Cards, y.Cards);
        }
    }

    class CardStringValueComparer(string cardDefinitions) : IComparer<string>
    {
        // allows us to alter the order of importance depending on whether jokers are in play
        private readonly string cardDefinitions = cardDefinitions;

        public int Compare(string? x, string? y)
        {
            if (x == null || y == null) throw new NotSupportedException("can't compare null hands");
            if (x.Length != y.Length) throw new InvalidOperationException($"can't compare hands with different sizes; first hand has {x.Length} cards, second has {y.Length}");

            // now fall back to plain old "strength"
            for (int i = 0; i < x.Length; i++)
            {
                var xStrength = cardDefinitions.IndexOf(x[i]);
                var yStrength = cardDefinitions.IndexOf(y[i]);
                if (xStrength == yStrength) continue;

                return xStrength - yStrength;
            }
            return 0; // if they literally have the same hand, it's a draw
        }
    }

    enum HandType : int
    {
        Invalid = 0,
        HighCard = 1,
        OnePair = 2,
        TwoPair = 3,
        ThreeOfAKind = 4,
        FullHouse = 5,
        FourOfAKind = 6,
        FiveOfAKind = 7,
    }

    static HandType DetermineHandType(string cards)
    {
        var countPerCard = CardCountsByType(cards);
        return CardCountsByTypeToHandType(countPerCard);
    }

    static string HandRepresentation(string cards)
    {
        var orderedChars = cards.Order().ToArray();

        var counts = new List<char>();

        // this builds an unsorted list of unique character counts into "counts"
        // e.g. 233QK will result in 1211
        var prevChar = orderedChars[0];
        var currentCharCount = '1';
        for (int i = 1; i < orderedChars.Length; i++)
        {
            if (orderedChars[i] == prevChar)
            {
                currentCharCount++;
            }
            else // a different character
            {
                counts.Add(currentCharCount);
                prevChar = orderedChars[i];
                currentCharCount = '1';
            }
        }
        // don't forget the last element
        counts.Add(currentCharCount);

        // sort it so the biggest number is first e.g. 2111
        return new string(counts.OrderDescending().ToArray());
    }

    // try a different, perhaps more intuitive way of determining hand type
    static HandType DetermineHandTypeAlt(string cards)
    {
        return HandRepresentation(cards) switch
        {
            "5" => HandType.FiveOfAKind,
            "41" => HandType.FourOfAKind,
            "32" or "23" => HandType.FullHouse,
            "311" => HandType.ThreeOfAKind,
            "221" => HandType.TwoPair,
            "2111" => HandType.OnePair,
            "11111" => HandType.HighCard,
            _ => throw new InvalidOperationException($"Unhandled representation {countRepresentation} for cards {cards}")
        };
    }

    static HandType DetermineHandTypeUsingJokers(string cards)
    {
        var cardsWithoutJokers = new string(cards.Where(ch => ch != 'J').ToArray());
        var numJokers = cards.Length - cardsWithoutJokers.Length;

        var countPerCard = CardCountsByType(cardsWithoutJokers);
        if (numJokers > 0) // we have some jokers, reallocate them to the next best thing
        {
            if (numJokers == 5) return HandType.FiveOfAKind; // short-circuit, we don't need to do anything else

            var orderedKeyValuePairs = countPerCard.OrderByDescending(kvp => kvp.Value).ToArray();

            char strongestCard;
            // if (orderedKeyValuePairs.Length == 0) => 5 jokers, this will be 5-of-a-kind, handled naturally by the regular algorithm later
            if (orderedKeyValuePairs.Length == 1) // 4 jokers and one other thing
            {
                strongestCard = orderedKeyValuePairs[0].Key;
            }
            else if (orderedKeyValuePairs[0].Value > orderedKeyValuePairs[1].Value)
            {
                // more of A than B, add to A
                strongestCard = orderedKeyValuePairs[0].Key;
            }
            else
            {
                // either we have two pairs, or a bunch of single cards
                strongestCard = cardsWithoutJokers.MaxBy(CardDefinitionsWithJoker.IndexOf);
            }

            countPerCard[strongestCard] = countPerCard[strongestCard] + numJokers;
        }
        return CardCountsByTypeToHandType(countPerCard);
    }

    private static Dictionary<char, int> CardCountsByType(string cards)
    {
        var result = new Dictionary<char, int>(capacity: cards.Length);
        foreach (var ch in cards)
        {
            if (result.TryGetValue(ch, out var prevCount))
            {
                result[ch] = prevCount + 1;
            }
            else
            {
                result[ch] = 1;
            }
        }
        return result;
    }

    private static HandType CardCountsByTypeToHandType(Dictionary<char, int> countPerCard)
    {
        // countPerCard.Count is the number of unique card types in the hand
        return countPerCard.Count switch
        {
            // if there's only one card type, all 5 cards must be it
            1 => HandType.FiveOfAKind,

            // if there's two card types, must be four of a kind or full house
            2 => countPerCard.Values.Max() switch
            {
                4 => HandType.FourOfAKind,
                3 => HandType.FullHouse,
                _ => throw new InvalidOperationException($"Unexpected: 2 unique card types; max count of any one type is {countPerCard.Values.Max()}")
            },

            3 => countPerCard.Values.Max() switch
            {
                3 => HandType.ThreeOfAKind,
                2 => HandType.TwoPair,
                _ => throw new InvalidOperationException($"Unexpected: 3 unique card types; max count of any one type is {countPerCard.Values.Max()}")
            },

            4 => HandType.OnePair,

            5 => HandType.HighCard,

            _ => throw new InvalidOperationException($"Unexpected: {countPerCard.Count} unique card types"),
        };
    }

    record Hand(HandType HandType, string Cards, int Bid);
}