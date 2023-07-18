

using Newtonsoft.Json;

namespace AcesCore
{
    // When the player is done playing for the round, whether that be because they went out,
    // or because someone else went out and their turn came around.
    public class PlayerDoneForRound : Event
    {
        public override EventType Type => EventType.PlayerDoneForRound;

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("roundScore")]
        public int RoundScore { get; set; }

        [JsonProperty("totalScore")]
        public int TotalScore { get; set; }

        public PlayerDoneForRound(string displayName, int roundScore, int totalScore)
        {
            DisplayName = displayName;
            RoundScore = roundScore;
            TotalScore = totalScore;
        }

        public PlayerDoneForRound()
        {
        }
    }
}