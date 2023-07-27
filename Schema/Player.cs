using System.Collections.Generic;
using AcesCore;
using Newtonsoft.Json;

namespace Schema
{
    public class Player
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("hand")]
        public List<Card> Hand;

        [JsonProperty("score")]
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