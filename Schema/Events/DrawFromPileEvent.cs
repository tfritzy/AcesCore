using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AcesCore
{
    public class DrawFromPileEvent : Event
    {
        public override EventType Type => EventType.DrawFromPile;

        [JsonProperty("player")]
        public string Player;

        public DrawFromPileEvent(string displayName)
        {
            Player = displayName;
        }

        public DrawFromPileEvent()
        {
        }
    }
}