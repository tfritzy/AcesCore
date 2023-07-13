using System.Text.Json.Serialization;
using AcesCore;

namespace AcesCore
{
    public class PlayerWentOutEvent : Event
    {
        public override EventType Type => EventType.PlayerWentOut;

        [JsonPropertyName("player")]
        public string Player;

        public PlayerWentOutEvent(string displayName)
        {
            Player = displayName;
        }
    }
}