using AcesCore;
using Newtonsoft.Json;

namespace AcesCore
{
    public class DiscardEvent : Event
    {
        public override EventType Type => EventType.Discard;

        [JsonProperty("playerId")]
        public string PlayerId;

        [JsonProperty("card")]
        public Card Card;

        public DiscardEvent(string playerId, Card card)
        {
            PlayerId = playerId;
            Card = card;
        }

        public DiscardEvent()
        {
        }
    }
}