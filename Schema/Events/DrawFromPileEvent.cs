using System.Text.Json.Serialization;

namespace AcesCore
{
    public class DrawFromPileEvent : Event
    {
        public override EventType Type => EventType.DrawFromPile;

        [JsonPropertyName("player")]
        public string Player;

        public DrawFromPileEvent(string displayName)
        {
            Player = displayName;
        }
    }
}