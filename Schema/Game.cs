using AcesCore;

namespace Schema
{
    public class Game
    {
        public string Id;
        public List<Player> Players;
        public List<Card> Deck;
        public List<Card> Pile;
        public int Round;
        public int TurnIndex;
        public GameState State;
        public int NumRounds;

        public Game(string id)
        {
            Id = id;
            Players = new();
            Deck = new();
            Pile = new();
            Round = 0;
            NumRounds = 10;
            State = GameState.Setup;
        }
    }
}
