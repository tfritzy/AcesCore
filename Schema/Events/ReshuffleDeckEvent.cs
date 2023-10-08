using System.Collections.Generic;
using System.Text.Json.Serialization;
using AcesCore;
using Newtonsoft.Json;

namespace AcesCore
{
    public class ReshuffleDeckEvent : Event
    {
        public override EventType Type => EventType.ReshuffleDeck;

        [JsonProperty("deckSize")]
        public int DeckSize;

        [JsonProperty("pile")]
        public List<Card> Pile;

        public ReshuffleDeckEvent(int deckSize, List<Card> pile)
        {
            DeckSize = deckSize;
            Pile = pile;
        }

        public ReshuffleDeckEvent()
        {
        }
    }
}