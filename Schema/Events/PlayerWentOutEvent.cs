using System.Text.Json.Serialization;
using AcesCore;
using Newtonsoft.Json;

namespace AcesCore
{
    public class PlayerWentOutEvent : Event
    {
        public override EventType Type => EventType.PlayerWentOut;

        [JsonProperty("player")]
        public string Player;

        public PlayerWentOutEvent(string displayName)
        {
            Player = displayName;
        }

        public PlayerWentOutEvent()
        {
        }
    }
}