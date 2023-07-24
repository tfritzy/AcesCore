using System.Text.Json.Serialization;
using AcesCore;
using Newtonsoft.Json;

namespace AcesCore
{
    public class DiscardEvent : Event
    {
        public override EventType Type => EventType.Discard;

        [JsonProperty("player")]
        public string Player;

        [JsonProperty("card")]
        public Card Card;

        public DiscardEvent(string displayName, Card card)
        {
            Player = displayName;
            Card = card;
        }

        public DiscardEvent()
        {
        }
    }
}