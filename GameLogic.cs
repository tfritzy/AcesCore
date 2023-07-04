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
                game.Deck.AddRange(Constants.FullDeck);
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

        public static bool CanGoOut(List<Card> cards, Card wild)
        {
            return true;
        }

        public static void GoOut(Game game, Player player)
        {
            AdvanceRound(game);
        }

        public static void AdvanceRound(Game game)
        {
            game.Round += 1;
            InitDeck(game);
            DealCards(game);
            game.TurnPhase = TurnPhase.Drawing;

            if (game.Round > game.NumRounds)
            {
                game.State = GameState.Finished;
            }
        }

        public static void AdvanceTurn(Game game)
        {
            game.TurnIndex += 1;
            game.TurnPhase = TurnPhase.Drawing;
        }

        public static Card DrawFromDeck(Game game, string playerId)
        {
            return DrawFrom(game, game.Deck, playerId);
        }

        public static Card DrawFromPile(Game game, string playerId)
        {
            return DrawFrom(game, game.Pile, playerId);
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

        public static void Discard(Game game, string playerId, Card card)
        {
            Player player = game.Players.Find((p) => p.Id == playerId) ?? throw new BadRequest("You don't exist.");
            int index = game.Players.IndexOf(player);

            if (index != game.TurnIndex)
                throw new BadRequest("It's not your turn.");

            if (player.Hand.Count - HandSizeForRound(game.Round) <= 0)
                throw new BadRequest("You have insufficient cards to discard one.");

            int cardIndex = player.Hand.IndexOf(card);

            if (cardIndex == -1)
                throw new BadRequest("You don't have that card.");

            player.Hand.RemoveAt(cardIndex);
            game.Pile.Add(card);
            game.TurnPhase = TurnPhase.Discarding;

            int extraCardCount = player.Hand.Count - HandSizeForRound(game.Round);
            if (extraCardCount <= 0)
            {
                AdvanceTurn(game);
            }
        }
    }
}
