

using Newtonsoft.Json;

namespace AcesCore
{
    public class JoinGameEvent : Event
    {
        public override EventType Type => EventType.JoinGame;

        [JsonProperty("playerId")]
        public string PlayerId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        public JoinGameEvent(string displayName, string playerId)
        {
            DisplayName = displayName;
            PlayerId = playerId;
        }

        public JoinGameEvent()
        {
        }
    }
}