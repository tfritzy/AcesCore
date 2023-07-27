using System.Collections.Generic;
using AcesCore;

namespace Schema
{
    public class Player
    {
        public string Id { get; private set; }
        public string DisplayName { get; set; }
        public List<Card> Hand;
        public int Score;
        public List<int> ScorePerRound;

        public Player(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
            ScorePerRound = new();
            Hand = new();
            Score = 0;
        }
    }
}