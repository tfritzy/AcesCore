using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AcesCore
{
    public class DrawFromDeckEvent : Event
    {
        public override EventType Type => EventType.DrawFromDeck;

        [JsonProperty("player")]
        public string Player;

        public DrawFromDeckEvent(string displayName)
        {
            Player = displayName;
        }

        public DrawFromDeckEvent()
        {
        }
    }
}