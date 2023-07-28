using System.Text.Json.Serialization;
using AcesCore;
using Newtonsoft.Json;

namespace AcesCore
{
    public class PlayerWentOutEvent : Event
    {
        public override EventType Type => EventType.PlayerWentOut;

        [JsonProperty("playerId")]
        public string PlayerId;

        public PlayerWentOutEvent(string playerId)
        {
            PlayerId = playerId;
        }

        public PlayerWentOutEvent()
        {
        }
    }
}