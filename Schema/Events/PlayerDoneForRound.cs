

using Newtonsoft.Json;

namespace AcesCore
{
    // When the player is done playing for the round, whether that be because they went out,
    // or because someone else went out and their turn came around.
    public class PlayerDoneForRound : Event
    {
        public override EventType Type => EventType.PlayerDoneForRound;

        [JsonProperty("playerId")]
        public string PlayerId { get; set; }

        [JsonProperty("roundScore")]
        public int RoundScore { get; set; }

        [JsonProperty("totalScore")]
        public int TotalScore { get; set; }

        [JsonProperty("groupedCards")]
        public List<List<Card>> GroupedCards { get; set; }

        [JsonProperty("ungroupedCards")]
        public List<Card> UngroupedCards;


        public PlayerDoneForRound(
            string playerId,
            int roundScore,
            int totalScore,
            List<List<Card>> groupedCards,
            List<Card> ungroupedCards)
        {
            PlayerId = playerId;
            RoundScore = roundScore;
            TotalScore = totalScore;
            GroupedCards = groupedCards;
            UngroupedCards = ungroupedCards;
        }

        public PlayerDoneForRound()
        {
        }
    }
}