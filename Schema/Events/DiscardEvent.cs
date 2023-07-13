using System.Text.Json.Serialization;
using AcesCore;

namespace AcesCore
{
    public class DiscardEvent : Event
    {
        public override EventType Type => EventType.Discard;

        [JsonPropertyName("player")]
        public string Player;

        [JsonPropertyName("card")]
        public Card Card;

        public DiscardEvent(string displayName, Card card)
        {
            Player = displayName;
            Card = card;
        }
    }
}