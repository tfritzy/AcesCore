using System.Text.Json.Serialization;

namespace AcesCore
{
    public class DrawFromDeckEvent : Event
    {
        public override EventType Type => EventType.DrawFromDeck;

        [JsonPropertyName("player")]
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