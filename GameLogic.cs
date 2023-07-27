using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schema;

namespace AcesCore
{
    public static class GameLogic
    {
        public const int INITIAL_HAND_SIZE = 3;

        public static Game CreateGame(GameSettings? settings = null)
        {
            Game game = new(id: IdGenerator.GenerateGameId(), settings);
            return game;
        }

        public static Player FindPlayer(Game game, string userId)
        {
            return game.Players.FirstOrDefault(player => player.Id == userId) ?? throw new BadRequest("You don't exist.");
        }

        public static int HandSizeForRound(int round)
        {
            return INITIAL_HAND_SIZE + round;
        }

        public static int MaxDrawable(Game game)
        {
            return game.Settings.Mining ? 3 : 1;
        }

        public static int NumDecksNeeded(int round, int numPlayers)
        {
            int numCardsHeld = HandSizeForRound(round) * numPlayers;

            // We want to there to be at least 20 cards
            // on the table at the start of the round.
            int numDecks = 0;
            while (numDecks * 54 < numCardsHeld + 20)
            {
                numDecks += 1;
            }

            return numDecks;
        }

        public static void InitDeck(Game game)
        {
            game.Pile = new();
            game.Deck = new();

            for (int i = 0; i < NumDecksNeeded(game.Round, game.Players.Count); i++)
            {
                game.Deck.AddRange(Constants.FullDeck.Select((type) => new Card(type, i)));
            }
        }

        public static void StartGame(Game game, string userId)
        {
            if (string.IsNullOrEmpty(userId) || game.Players.FirstOrDefault()?.Id != userId)
            {
                throw new BadRequest("Only the game owner can start the game.");
            }

            InitDeck(game);
            DealCards(game);
            game.State = GameState.Playing;
            game.TurnPhase = TurnPhase.Drawing;

            game.AddEvent(new StartGameEvent());
        }

        public static void DealCards(Game game)
        {
            foreach (Player player in game.Players)
            {
                player.Hand = new();
            }

            for (int i = 0; i < HandSizeForRound(game.Round); i++)
            {
                foreach (Player player in game.Players)
                {
                    player.Hand.Add(game.Deck.Last());
                    game.Deck.RemoveAt(game.Deck.Count - 1);
                }
            }

            game.Pile.Add(game.Deck.Last());
            game.Deck.RemoveAt(game.Deck.Count - 1);
        }

        public static void JoinGame(Game game, Player player)
        {
            if (game.Players.Select((p) => p.Id).Contains(player.Id))
            {
                throw new BadRequest("You're already in the game");
            }

            game.Players.Add(player);

            game.AddEvent(new JoinGameEvent(player.DisplayName));
        }

        public enum StreakType
        {
            None,
            StraightAsc,
            StraightDesc,
            Same
        }

        private static StreakType GetStreakType(Card cardA, Card cardB, CardValue wild)
        {
            if (IsWild(cardA, wild) || IsWild(cardB, wild))
            {
                return StreakType.None;
            }

            if (cardA.Value == cardB.Value)
            {
                return StreakType.Same;
            }

            if (IsInDirection(cardA, cardB, StepDir.Asc))
            {
                return StreakType.StraightAsc;
            }

            if (IsInDirection(cardA, cardB, StepDir.Desc))
            {
                return StreakType.StraightDesc;
            }

            return StreakType.None;
        }

        public enum StepDir
        {
            Asc,
            Desc
        };

        public static bool AreNStepApart(Card card1, Card card2, StepDir dir, int n)
        {
            if (card1.Suit != card2.Suit)
            {
                return false;
            }

            int neededDelta = dir == StepDir.Asc ? n : -n;

            int startValue =
                card1.Value == CardValue.Ace &&
                dir == StepDir.Asc
                    ? 0 : (int)card1.Value;

            int endValue =
                card2.Value == CardValue.Ace &&
                dir == StepDir.Desc
                    ? 0 : (int)card2.Value;

            return endValue - startValue == neededDelta;
        }

        public static bool IsInDirection(Card card1, Card card2, StepDir dir)
        {
            if (card1.Suit != card2.Suit)
            {
                return false;
            }

            int startValue =
                card1.Value == CardValue.Ace &&
                dir == StepDir.Asc
                    ? 0 : (int)card1.Value;

            int endValue =
                card2.Value == CardValue.Ace &&
                dir == StepDir.Desc
                    ? 0 : (int)card2.Value;

            int neededVal = dir == StepDir.Asc ? 1 : -1;
            return Math.Sign(endValue - startValue) == neededVal;
        }


        public static bool ContinuesStreak(Card prevCard, Card card, StreakType streakType, int gap, CardValue wild)
        {
            if (IsWild(card, wild) || IsWild(prevCard, wild))
            {
                return true;
            }

            if (streakType == StreakType.Same)
            {
                return prevCard.Value == card.Value;
            }

            if (streakType == StreakType.StraightAsc)
            {
                return AreNStepApart(prevCard, card, StepDir.Asc, gap);
            }

            if (streakType == StreakType.StraightDesc)
            {
                return AreNStepApart(prevCard, card, StepDir.Desc, gap);
            }

            return false;
        }

        private static bool IsWild(Card card, CardValue wild)
        {
            return card.Value == wild || card.Value == CardValue.Joker;
        }

        public static int[] GetGroupSizeAtIndex(List<Card> cards, CardValue wild)
        {
            int[] groupSizeAtIndex = new int[cards.Count];

            for (int i = 0; i < cards.Count - 1; i++)
            {
                int size = 1;
                StreakType streak = StreakType.None;
                int firstRealIndex = -1;

                while (i + size < cards.Count)
                {
                    int j = i + size;

                    if (!IsWild(cards[j - 1], wild) && firstRealIndex == -1)
                    {
                        firstRealIndex = j - 1;
                    }

                    if (streak == StreakType.None && firstRealIndex != -1)
                    {
                        streak = GetStreakType(cards[firstRealIndex], cards[j], wild);
                    }

                    if (ContinuesStreak(cards[j - 1], cards[j], streak, 1, wild) &&
                        (firstRealIndex == -1 || ContinuesStreak(cards[firstRealIndex], cards[j], streak, j - firstRealIndex, wild)))
                    {
                        size += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                groupSizeAtIndex[i] = size;
            }

            groupSizeAtIndex[cards.Count - 1] = 1;

            return groupSizeAtIndex;
        }

        public struct Group
        {
            public int Index;
            public int Size;
        }

        public static List<Group> GetBestGroups(int[] groupsPerIndex, int index, List<Group> groups, int[] best)
        {
            if (index >= groupsPerIndex.Length)
            {
                return groups;
            }

            int currentGrouped = groups.Sum((g) => g.Size);
            if (best[index] > currentGrouped)
            {
                return groups;
            }

            int mostGrouped = 0;
            List<Group> bestGroups = new(0);
            for (int i = groupsPerIndex[index]; i > 0; i--)
            {
                var groupClone = new List<Group>(groups);

                if (i > 2)
                {
                    groupClone.Add(new Group() { Index = index, Size = i });
                }

                var iGroups = GetBestGroups(groupsPerIndex, index + i, groupClone, best);
                int grouped = iGroups.Sum((g) => g.Size);
                if (grouped > mostGrouped)
                {
                    mostGrouped = grouped;
                    bestGroups = iGroups;
                }
            }

            best[index] = mostGrouped;

            return bestGroups;
        }

        public static CardValue GetWildForRound(int round)
        {
            if (round < 12)
            {
                return (CardValue)(round + 2);
            }

            return CardValue.Invalid;
        }

        private static bool AreCardsEquivalent(Player player, List<Card> cards)
        {
            if (player.Hand.Count != cards.Count)
            {
                return false;
            }

            foreach (Card card in cards)
            {
                if (!player.Hand.Contains(card))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CanGoOut(List<Card> cards, CardValue wild)
        {
            List<Group> bestGroups = GetBestGroups(
                groupsPerIndex: GetGroupSizeAtIndex(cards, wild),
                index: 0,
                new List<Group>(),
                new int[cards.Count]
            );

            return bestGroups.Sum((g) => g.Size) == cards.Count;
        }

        public static void GoOut(Game game, string playerId, List<Card> cards)
        {
            if (game.Players[game.TurnIndex].Id != playerId)
            {
                throw new BadRequest("It's not your turn.");
            }

            Player player = FindPlayer(game, playerId);

            if (!AreCardsEquivalent(player, cards))
            {
                throw new BadRequest("Your cards seem to be out of sync.");
            }

            int extraCardCount = player.Hand.Count - HandSizeForRound(game.Round);
            if (game.TurnPhase == TurnPhase.Drawing || extraCardCount > 0)
            {
                throw new BadRequest("You can't go out until you've drawn and discarded.");
            }

            if (!CanGoOut(cards, GetWildForRound(game.Round)))
            {
                throw new BadRequest("You can't go out with your current hand.");
            }

            game.PlayerWentOut ??= playerId;

            player.ScorePerRound.Add(0);
            game.AddEvent(
                new PlayerDoneForRound(
                    displayName: player.DisplayName,
                    roundScore: 0,
                    totalScore: player.Score));

            game.AddEvent(new PlayerWentOutEvent(player.DisplayName));
            AdvanceTurn(game);
        }

        public static void AdvanceRound(Game game)
        {
            game.Round += 1;
            InitDeck(game);
            DealCards(game);
            game.TurnIndex = game.Round % game.Players.Count;
            game.TurnPhase = TurnPhase.Drawing;
            game.PlayerWentOut = null;

            game.AddEvent(new AdvanceRoundEvent(game.Round));

            if (game.Round > game.NumRounds)
            {
                EndGame(game);
            }
        }

        public static void EndGame(Game game)
        {
            game.State = GameState.Finished;
            game.AddEvent(new GameEndEvent());
        }

        public static void AdvanceTurn(Game game)
        {
            game.TurnIndex += 1;
            game.TurnIndex %= game.Players.Count;
            game.TurnPhase = TurnPhase.Drawing;
            game.AddEvent(new AdvanceTurnEvent(game.TurnIndex));
        }

        public static Card DrawFromDeck(Game game, string playerId)
        {
            Player player = FindPlayer(game, playerId);
            Card card = DrawFrom(game, game.Deck, playerId);
            game.AddEvent(new DrawFromDeckEvent(player.DisplayName));
            return card;
        }

        public static Card DrawFromPile(Game game, string playerId)
        {
            Player player = FindPlayer(game, playerId);
            Card card = DrawFrom(game, game.Pile, playerId);
            game.AddEvent(new DrawFromPileEvent(player.DisplayName));
            return card;
        }

        private static Card DrawFrom(Game game, List<Card> cards, string playerId)
        {
            Player? player = game.Players.Find((p) => p.Id == playerId) ?? throw new BadRequest("You don't exist.");
            int index = game.Players.IndexOf(player);

            if (index != game.TurnIndex)
                throw new BadRequest("It's not your turn.");

            int numDrawn = player.Hand.Count - HandSizeForRound(game.Round);
            if (numDrawn >= MaxDrawable(game))
                throw new BadRequest("You can't draw any more cards. You need to discard now.");

            if (game.TurnPhase != TurnPhase.Drawing)
                throw new BadRequest("You can't draw after you start discarding.");

            if (cards.Count == 0)
                throw new BadRequest("You can't draw from an empty pile.");

            Card card = cards.Last();
            cards.RemoveAt(cards.Count - 1);
            player.Hand.Add(card);

            if (numDrawn + 1 >= MaxDrawable(game))
            {
                game.TurnPhase = TurnPhase.Discarding;
            }

            return card;
        }

        public static Card Discard(Game game, string playerId, Card card)
        {
            Player player = game.Players.Find((p) => p.Id == playerId) ?? throw new BadRequest("You don't exist.");
            int index = game.Players.IndexOf(player);

            if (index != game.TurnIndex)
                throw new BadRequest("It's not your turn.");

            if (player.Hand.Count - HandSizeForRound(game.Round) <= 0)
                throw new BadRequest("You have insufficient cards to discard one.");

            int cardIndex = player.Hand.FindIndex((c) => c == card);

            if (cardIndex == -1)
                throw new BadRequest("You don't have that card.");

            player.Hand.RemoveAt(cardIndex);
            game.Pile.Add(card);
            game.TurnPhase = TurnPhase.Discarding;

            game.AddEvent(new DiscardEvent(player.DisplayName, card));

            return card;
        }

        public static void EndTurn(Game game, string playerId)
        {
            Player player = game.Players.Find((p) => p.Id == playerId) ?? throw new BadRequest("You don't exist.");

            if (game.Players[game.TurnIndex]?.Id != playerId)
                throw new BadRequest("It's not your turn.");

            int extraCardCount = player.Hand.Count - HandSizeForRound(game.Round);
            if (extraCardCount > 0)
            {
                throw new BadRequest("You can't end your turn until you've discarded.");
            }

            if (game.TurnPhase == TurnPhase.Drawing)
            {
                throw new BadRequest("You can't end your turn before you've drawn.");
            }

            if (!string.IsNullOrEmpty(game.PlayerWentOut))
            {
                int roundScore = 0; // TODO: Calculate

                if (!CanGoOut(player.Hand, GetWildForRound(game.Round)))
                {
                    roundScore = 1;
                    player.Score += roundScore;
                }
                player.ScorePerRound.Add(roundScore);

                game.AddEvent(
                    new PlayerDoneForRound(
                        displayName: player.DisplayName,
                        roundScore: roundScore,
                        totalScore: player.Score));

            }

            AdvanceTurn(game);

            if (game.PlayerWentOut != null && game.Players[game.TurnIndex].Id == game.PlayerWentOut)
            {
                AdvanceRound(game);
            }
        }
    }
}
