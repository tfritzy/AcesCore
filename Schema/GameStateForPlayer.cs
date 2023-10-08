using System.Collections.Generic;
using System.Linq;
using Schema;

namespace AcesCore
{
    public class GameStateForPlayer
    {
        public List<Card> hand { get; set; }
        public int deckSize { get; set; }
        public List<Card> pile { get; set; }
        public List<PlayerState> players { get; set; }
        public int turn { get; set; }
        public TurnPhase turnPhase { get; set; }
        public int round { get; set; }
        public GameState state { get; set; }

        public GameStateForPlayer(Game game, string token)
        {
            Player player = game.Players.Find(p => p.Token == token);

            if (player == null)
            {
                return;
            }

            hand = player.Hand;
            deckSize = game.Deck.Count;
            pile = game.Pile;
            turn = game.TurnIndex;
            turnPhase = game.TurnPhase;
            round = game.Round;
            state = game.State;
            players = game.Players.Select(p =>
            {
                int mostRecentCompletedRound = p.HandHistory.Count - 1;
                var groups = GameLogic.GetCardGroups(
                    p.HandHistory.LastOrDefault() ?? new List<Card>(),
                    GameLogic.GetWildForRound(mostRecentCompletedRound));

                return new PlayerState
                {
                    displayName = p.DisplayName,
                    id = p.Id,
                    score = p.Score,
                    scorePerRound = p.ScorePerRound,
                    mostRecentGroupedCards = groups.GroupedCards,
                    mostRecentUngroupedCards = groups.UngroupedCards,
                };
            }
            ).ToList();
        }

        public class PlayerState
        {
            public string id { get; set; }
            public string displayName { get; set; }
            public int score;
            public List<int> scorePerRound;
            public List<List<Card>> mostRecentGroupedCards;
            public List<Card> mostRecentUngroupedCards;
        }
    }
}