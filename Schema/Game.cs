using System.Collections.Generic;
using AcesCore;

namespace Schema
{
    public enum TurnPhase
    {
        Invalid,
        Drawing,
        Discarding,
        Ending,
    }

    public class Game
    {
        public string id;
        public List<Player> Players;
        public List<Card> Deck;
        public List<Card> Pile;
        public int Round;
        public int TurnIndex;
        public GameState State;
        public TurnPhase TurnPhase;
        public int NumRounds;
        public GameSettings Settings;
        private List<Event> Events;
        public string? PlayerWentOutId;

        public Game(string id, GameSettings? settings = null)
        {
            this.id = id;
            Players = new();
            Deck = new();
            Pile = new();
            Events = new();
            Round = 0;
            NumRounds = 12;
            State = GameState.Setup;
            TurnPhase = TurnPhase.Invalid;
            Settings = settings ?? new();
            PlayerWentOutId = null;
        }

        public List<Event> GetEvents()
        {
            return Events;
        }

        public void AddEvent(Event e)
        {
            e.I = Events.Count;
            Events.Add(e);
        }
    }
}
