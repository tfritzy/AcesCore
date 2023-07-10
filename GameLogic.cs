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

            game.Events.Add(new StartGameEvent());
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
        }

        public enum StreakType
        {
            None,
            StraightAsc,
            StraightDesc,
            Same
        }

        public enum StepDir
        {
            Asc,
            Desc
        };

        public static bool AreOneStepApart(Card card1, Card card2, StepDir dir)
        {
            if (card1.Suit != card2.Suit)
            {
                return false;
            }

            if (dir == StepDir.Asc && card1.Value == CardValue.Ace && card2.Value == CardValue.Two)
            {
                return true;
            }

            if (dir == StepDir.Desc && card1.Value == CardValue.Two && card2.Value == CardValue.Ace)
            {
                return true;
            }

            int neededDelta = dir == StepDir.Asc ? 1 : -1;

            return (int)card2.Value - (int)card1.Value == neededDelta;
        }

        public static bool ContinuesStreak(Card card, Card? lastCard, StreakType streakType)
        {
            if (lastCard == null)
            {
                return true;
            }

            if (streakType == StreakType.None)
            {
                return lastCard.Value == card.Value ||
                       AreOneStepApart(lastCard, card, StepDir.Asc) ||
                       AreOneStepApart(lastCard, card, StepDir.Desc);
            }

            if (streakType == StreakType.Same)
            {
                return lastCard.Value == card.Value;
            }

            if (streakType == StreakType.StraightAsc)
            {
                return AreOneStepApart(lastCard, card, StepDir.Asc);
            }

            if (streakType == StreakType.StraightDesc)
            {
                return AreOneStepApart(lastCard, card, StepDir.Desc);
            }

            return false;
        }

        public static bool CanGoOut(List<Card> cards, Card wild)
        {
            return true;
        }

        public static void GoOut(Game game, string playerId)
        {
            if (game.Players[game.TurnIndex].Id != playerId)
            {
                throw new BadRequest("It's not your turn.");
            }

            Player player = FindPlayer(game, playerId);

            int extraCardCount = player.Hand.Count - HandSizeForRound(game.Round);
            if (game.TurnPhase == TurnPhase.Drawing || extraCardCount > 0)
            {
                throw new BadRequest("You can't go out until you've drawn and discarded.");
            }

            game.PlayerWentOut ??= playerId;

            game.Events.Add(new PlayerWentOutEvent(player.DisplayName));
            AdvanceTurn(game);
        }

        public static void AdvanceRound(Game game)
        {
            game.Round += 1;
            InitDeck(game);
            DealCards(game);
            game.TurnIndex = game.Round % game.Players.Count;
            game.TurnPhase = TurnPhase.Drawing;

            game.Events.Add(new AdvanceRoundEvent());

            if (game.Round > game.NumRounds)
            {
                EndGame(game);
            }
        }

        public static void EndGame(Game game)
        {
            game.State = GameState.Finished;
            game.Events.Add(new GameEndEvent());
        }

        public static void AdvanceTurn(Game game)
        {
            game.TurnIndex += 1;
            game.TurnIndex %= game.Players.Count;
            game.TurnPhase = TurnPhase.Drawing;
            game.Events.Add(new AdvanceTurnEvent());
        }

        public static Card DrawFromDeck(Game game, string playerId)
        {
            Player player = FindPlayer(game, playerId);
            Card card = DrawFrom(game, game.Deck, playerId);
            game.Events.Add(new DrawFromDeckEvent(player.DisplayName));
            return card;
        }

        public static Card DrawFromPile(Game game, string playerId)
        {
            Player player = FindPlayer(game, playerId);
            Card card = DrawFrom(game, game.Pile, playerId);
            game.Events.Add(new DrawFromPileEvent(player.DisplayName));
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

        public static void Discard(Game game, string playerId, CardType cardType)
        {
            Player player = game.Players.Find((p) => p.Id == playerId) ?? throw new BadRequest("You don't exist.");
            int index = game.Players.IndexOf(player);

            if (index != game.TurnIndex)
                throw new BadRequest("It's not your turn.");

            if (player.Hand.Count - HandSizeForRound(game.Round) <= 0)
                throw new BadRequest("You have insufficient cards to discard one.");

            int cardIndex = player.Hand.FindIndex((c) => c.Type == cardType);

            if (cardIndex == -1)
                throw new BadRequest("You don't have that card.");

            Card card = player.Hand[cardIndex];
            player.Hand.RemoveAt(cardIndex);
            game.Pile.Add(card);
            game.TurnPhase = TurnPhase.Discarding;

            game.Events.Add(new DiscardEvent(player.DisplayName, card));
        }

        public static void EndTurn(Game game, string playerId)
        {
            Player player = game.Players.Find((p) => p.Id == playerId) ?? throw new BadRequest("You don't exist.");

            if (game.Players[game.TurnIndex]?.Id != playerId)
                throw new BadRequest("It's not your turn.");

            int extraCardCount = player.Hand.Count - HandSizeForRound(game.Round);
            if (extraCardCount > 0)
            {
                throw new BadRequest("You can't end your turn until you have discarded.");
            }

            AdvanceTurn(game);

            if (game.PlayerWentOut != null && game.Players[game.TurnIndex].Id == game.PlayerWentOut)
            {
                AdvanceRound(game);
            }
        }
    }
}
