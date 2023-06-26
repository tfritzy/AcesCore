using System.Text;
using Schema;

namespace AcesCore
{
    public static class GameLogic
    {
        public static Game CreateGame()
        {
            Game game = new(id: IdGenerator.GenerateGameId());
            return game;
        }

        public static int NumDecksNeeded(int round, int numPlayers)
        {
            int numCardsHeld = (round + 3) * numPlayers;

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

        public static void StartGame(Game game)
        {
            InitDeck(game);
            DealCards(game);
            game.State = GameState.Playing;
        }

        public static void DealCards(Game game)
        {
            foreach (Player player in game.Players)
            {
                player.Hand = new();
            }

            for (int i = 0; i < game.Round + 3; i++)
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
                return;
            }

            game.Players.Add(player);
        }

        public static Player CreatePlayer()
        {
            return new Player(id: IdGenerator.GenerateGenericId("plyr"));
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

            if (game.Round > game.NumRounds)
            {
                game.State = GameState.Finished;
            }
        }

        public static void DrawFromDeck(Game game, string playerId)
        {
            DrawFrom(game, game.Deck, playerId);
        }

        public static void DrawFromPile(Game game, string playerId)
        {
            DrawFrom(game, game.Pile, playerId);
        }

        private static void DrawFrom(Game game, List<Card> cards, string playerId)
        {
            Player? player = game.Players.Find((p) => p.Id == playerId) ?? throw new BadRequest("You don't exist");
            int index = game.Players.IndexOf(player);

            if (index != game.TurnIndex)
                throw new BadRequest("It's not your turn");

            Card card = cards.Last();
            cards.RemoveAt(cards.Count - 1);
            player.Hand.Add(card);
        }

        public static void Discard(Game game, string playerId, Card card)
        {
            Player player = game.Players.Find((p) => p.Id == playerId) ?? throw new BadRequest("You don't exist");
            int index = game.Players.IndexOf(player);

            if (index != game.TurnIndex)
                throw new BadRequest("It's not your turn");

            if (player.Hand.Count - (game.Round + 3) <= 0)
                throw new BadRequest("You have insufficient cards to discard one");

            int cardIndex = player.Hand.IndexOf(card);

            if (cardIndex == -1)
                throw new BadRequest("You don't have that card");

            player.Hand.RemoveAt(cardIndex);
            game.Pile.Add(card);
        }
    }
}
